using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Master.Config;
using Master.Data;
using Master.DTOs;
using Master.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Master.Services;

/// <summary>
/// Service implementation for handling authentication-related business logic.
/// This class manages user registration, credential verification, and JWT generation.
/// </summary>
public class AuthService : IAuthService
{

    private const string RefreshTokenType = "refresh";
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly MasterDbContext _context;
    private readonly JwtConfig _config;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public Guid UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            return Guid.Parse(userId);
        }
    }

    /// <summary>
    /// Constructor for AuthService, utilizing dependency injection to receive necessary services.
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="context"></param>
    /// <param name="config"></param>
    /// <param name="mapper"></param>
    /// <param name="roleManager"></param>
    public AuthService(
        UserManager<AppUser> userManager,
        MasterDbContext context,
        IOptions<JwtConfig> config,
        IMapper mapper,
        RoleManager<IdentityRole<Guid>> roleManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _context = context;
        _config = config.Value;
        _mapper = mapper;
        _roleManager = roleManager;
        _httpContextAccessor = httpContextAccessor;
    }


    /// <summary>
    /// Authenticates a user based on provided credentials and generates a session token.
    /// </summary>
    /// <remarks>
    /// The process involves:
    /// 1. Locating the user by email.
    /// 2. Verifying the hashed password.
    /// 3. Generating a JWT if the credentials are valid.
    /// </remarks>
    /// <param name="request">The login request containing email and password.</param>
    /// <returns>An <see cref="AuthResponseDTO"/> containing user details and the generated access token.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid.</exception>
    public async Task<AuthResponseDTO> LoginUserAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await GenerateTokensAsync(user);
    }

    /// <summary>
    /// Generates JWT access and refresh tokens for a given user, including role claims and token expiration details.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private async Task<AuthResponseDTO> GenerateTokensAsync(AppUser user)
    {
        var accessToken = await GenerateAccessTokenAsync(user, _config.ExpirationMinutes);
        var (refreshEntity, refreshjwt) = await CreateRefreshTokenJwtAsync(user.Id.ToString(), _config.RefreshTokenExpirationDays);
        var roles = await _userManager.GetRolesAsync(user);

        var response = _mapper.Map<AuthResponseDTO>(user);

        response.AccessToken = accessToken;
        response.RefreshToken = refreshjwt;
        response.ExpiresAt = DateTime.UtcNow.AddMinutes(_config.ExpirationMinutes);
        response.RefreshTokenExpiresAt = refreshEntity.ExpiresAt;
        response.Roles = roles.ToList();

        return response;
    }

    /// <summary>
    /// Creates a refresh token JWT for a specific user, storing the token's unique identifier (JTI) and expiration in the database for later validation and revocation.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="expirationDays"></param>
    /// <returns></returns>
    private async Task<(RefreshToken entity, string jwt)> CreateRefreshTokenJwtAsync(string userId, int expirationDays)
    {
        var jti = Guid.NewGuid().ToString();
        var expiresAt = DateTime.UtcNow.AddDays(expirationDays);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.RefreshTokenSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim("token_type", RefreshTokenType)
        };

        var token = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);

        var entity = new RefreshToken
        {
            JwtId = jti,
            UserId = userId,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
        _context.RefreshTokens.Add(entity);
        await _context.SaveChangesAsync();

        return (entity, jwtString);
    }

    /// <summary>
    /// Generates a JWT access token for the specified user, embedding necessary claims such as user ID, email, and roles, and signing it with the configured secret key.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="accessExpirationMinutes"></param>
    /// <returns></returns>
    private async Task<string> GenerateAccessTokenAsync(AppUser user, int accessExpirationMinutes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email,user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(accessExpirationMinutes),
            signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Refreshes the access token using a valid refresh token, ensuring the refresh token is active and belongs to the correct user before generating new tokens.
    /// </summary>
    /// <param name="refreshTokenRequest"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task<AuthResponseDTO?> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        var (principal, jti) = ValidateRefreshJwtAndGetJti(refreshTokenRequest.RefreshToken);

        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jti);
        if (storedToken is null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        if (!storedToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token is no longer active.");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
    ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            throw new UnauthorizedAccessException("User not found.");

        storedToken.RevokedAt = DateTimeOffset.UtcNow;

        var newTokens = await GenerateTokensAsync(user);
        var newStored = await _context.RefreshTokens
             .FirstOrDefaultAsync(rt => rt.JwtId == GetJtiFromRefreshToken(newTokens.RefreshToken));
        if (newStored is not null)
            storedToken.ReplacedByJwtId = newStored.JwtId;

        await _context.SaveChangesAsync();
        return newTokens;
    }

    /// <summary>
    /// Validates the provided refresh token JWT, ensuring it is well-formed, signed with the correct key, and contains the expected claims.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="validateLifetime"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    private (ClaimsPrincipal principal, string jti) ValidateRefreshJwtAndGetJti(string refreshToken, bool validateLifetime = true)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.RefreshTokenSecretKey));

        var principal = handler.ValidateToken(refreshToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = _config.Issuer,
            ValidateAudience = true,
            ValidAudience = _config.Audience,
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwt)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        var tokenType = jwt.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;
        if (tokenType != RefreshTokenType)
            throw new UnauthorizedAccessException("Invalid token type.");

        var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        return (principal, jti);
    }

    /// <summary>
    /// Handles the creation of a new user account in the system.
    /// </summary>
    /// <remarks>
    /// The process involves:
    /// 1. Checking if the email is already registered.
    /// 2. Mapping the DTO to the AppUser entity.
    /// 3. Persisting the user to the database via Identity UserManager.
    /// </remarks>
    /// <param name="request">The registration data provided by the user.</param>
    /// <returns>An <see cref="AuthResponseDTO"/> indicating successful registration and initial login state.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user already exists or identity creation fails.</exception>
    public async Task<AuthResponseDTO> RegisterUserAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        var user = _mapper.Map<AppUser>(request);

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new InvalidOperationException($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        string roleToAssign = request.Role switch
        {
            "Master" => "Master",
            _ => "Customer"
        };

        if (!await _roleManager.RoleExistsAsync(roleToAssign))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(roleToAssign));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, roleToAssign);

        if (!roleResult.Succeeded)
            throw new InvalidOperationException("Failed to assign role to user.");

        return await GenerateTokensAsync(user);
    }

    /// <summary>
    /// Revokes a refresh token, marking it as inactive and preventing any future use for obtaining new access tokens.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        string? jti;
        try
        {
            (_, jti) = ValidateRefreshJwtAndGetJti(refreshToken, validateLifetime: false);
        }
        catch (Exception)
        {
            return;
        }

        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jti);
        if (storedToken is null || !storedToken.IsActive)
            return;

        storedToken.RevokedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets the JTI (JWT ID) claim value from a refresh token without validating its lifetime.
    /// </summary>
    /// <param name="refreshJwt"></param>
    /// <returns></returns>
    private static string GetJtiFromRefreshToken(string refreshJwt)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(refreshJwt))
            return string.Empty;
        var jwt = handler.ReadJwtToken(refreshJwt);
        return jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Updates the profile information of an existing user.
    /// </summary>
    /// <remarks>
    /// This method maps the <see cref="ProfileEditRequest"/> to the <see cref="AppUser"/> entity,
    /// performs the update via Identity UserManager, and returns a new set of tokens 
    /// to reflect any changes in user claims (e.g., Email or Name).
    /// </remarks>
    /// <param name="id"></param>
    /// <param name="request">The object containing updated user profile data.</param>
    /// <returns>An <see cref="AuthResponseDTO"/> containing the updated user details and new session tokens.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user is not found or the update process fails.</exception>
    public async Task<AuthResponseDTO> EditOwnProfileAsync(Guid id, ProfileEditRequest request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            throw new InvalidOperationException("User not found.");

        _mapper.Map(request, user);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Profile update failed: {errors}");
        }

        return await GenerateTokensAsync(user);
    }

    /// <summary>
    /// Changes the password for a specific user after verifying the current password.
    /// </summary>
    /// <remarks>
    /// The process involves:
    /// 1. Verifying the current password.
    /// 2. Validating the new password against Identity security policies.
    /// 3. Updating the security stamp to invalidate old sessions.
    /// 4. Generating new authentication tokens.
    /// </remarks>
    /// <param name="id"></param>
    /// <param name="request">The request containing current password, new password, and confirmation.</param>
    /// <returns>An <see cref="AuthResponseDTO"/> with new session tokens.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user is not found or the password change is rejected by Identity.</exception>
    public async Task<AuthResponseDTO> ChangePasswordAsync(Guid id, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            throw new InvalidOperationException("User not found.");

        var checkCurrentPassword = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
        if (!checkCurrentPassword)
        {
            throw new InvalidOperationException("CheckPasswordAsync failed: Current password is wrong.");
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Identity error: {errors}");
        }

        return await GenerateTokensAsync(user);
    }

    /// <summary>
    /// Retrieves the user entity with the specified identifier, including related skills and job posts.
    /// </summary>
    /// <remarks>The returned user entity includes related skills and job posts. This method performs a
    /// database query and may incur network or I/O latency.</remarks>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user entity associated with the
    /// specified identifier.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no user with the specified identifier exists.</exception>
    public async Task<AppUser> GetUserEntity(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.UserSkills)
                .ThenInclude(us => us.Skill)
            .Include(u => u.JobPosts)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            throw new KeyNotFoundException($"User with ID {id} was not found.");

        return user;
    }

    public async Task<bool> DeleteOwnProfile(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.UserSkills)
            .Include(u => u.JobPosts)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            throw new KeyNotFoundException("User profile not found.");

        if (user.UserSkills.Any()) _context.UserSkills.RemoveRange(user.UserSkills);
        if (user.JobPosts.Any()) _context.JobPosts.RemoveRange(user.JobPosts);

        await DeleteUserRefreshTokensAsync(id);

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to delete user: {errors}");
        }

        await _context.SaveChangesAsync();

        return true;
    }

    private async Task DeleteUserRefreshTokensAsync(Guid userId)
    {
        var tokens = _context.Set<RefreshToken>().Where(t => t.UserId == userId.ToString());

        if (await tokens.AnyAsync())
        {
            _context.Set<RefreshToken>().RemoveRange(tokens);
        }
    }

}