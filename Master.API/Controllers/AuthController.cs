using System.Security.Claims;
using Master.Application.Common;
using Master.Application.DTOs;
using Master.Application.Features.Authorization.Commands.ChangePassword;
using Master.Application.Features.Authorization.Commands.DeleteProfile;
using Master.Application.Features.Authorization.Commands.EditProfile;
using Master.Application.Features.Authorization.Commands.LoginUser;
using Master.Application.Features.Authorization.Commands.RefreshToken;
using Master.Application.Features.Authorization.Commands.RegisterUser;
using Master.Application.Features.Authorization.Commands.RevokeToken;
using Master.Application.Features.Authorization.Queries.GetUserById;
using Master.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Master.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Safely retrieves the authenticated user's ID from claims.
    /// </summary>
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User is not authenticated."));

    /// <summary>
    /// Registers a new user account within the system. This endpoint accepts user registration details such as email, password, and other relevant information
    /// Upon successful registration, it returns an authentication response containing access and refresh tokens for the newly created account.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Register([FromBody] RegisterRequest request)
    {
        var result = await _mediator.Send(new RegisterUserCommand(request));
        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "Registration completed successfully."));
    }

    /// <summary>
    /// Logins a user by validating their credentials and returns JWT tokens. This endpoint accepts the 
    /// user's email and password, verifies them against the stored credentials,
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(new LoginUserCommand(request));

        Response.Cookies.Append("X-Access-Token", result.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        });

        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "Login successful."));
    }

    /// <summary>
    /// Gets the authenticated user's profile information. This endpoint retrieves the user's details such as name, email, address, and other relevant information based on their unique identifier (userId).
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost("id/{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AppUser>>> GetById(Guid userId)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(userId));
        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "User retrieved successfully."));
    }

    /// <summary>
    /// Refreshes the access token using a valid refresh token. This endpoint accepts a refresh token, validates it, 
    /// and if valid, issues a new access token along with a new refresh token.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request));
        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result));
    }

    /// <summary>
    /// Revokes a refresh token, effectively logging the user out from all sessions that rely on that token.
    /// This endpoint accepts a refresh token and invalidates it in the system,
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("revoke")]
    public async Task<ActionResult> Revoke([FromBody] RefreshTokenRequest request)
    {
        await _mediator.Send(new RevokeTokenCommand(request.RefreshToken));
        return Ok(ApiResponse<string>.SuccessResponse("Refresh token revoked successfully."));
    }

    /// <summary>
    /// Edits the profile information of the currently authenticated user. This endpoint 
    /// allows users to update their personal details such as name, email, address, and phone number.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("editProfile")]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> EditOwnProfile([FromBody] ProfileEditRequest request)
    {
        var result = await _mediator.Send(new EditProfileCommand(UserId, request));
        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "Profile updated successfully."));
    }

    /// <summary>
    /// changes the password of the currently authenticated user. This endpoint requires the user to provide their current password for verification,
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("changePassword")]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> ChangeOwnPassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _mediator.Send(new ChangePasswordCommand(UserId, request));
        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "Password changed successfully."));
    }

    /// <summary>
    /// Deletes the authenticated user's own profile and all associated data permanently. 
    /// This endpoint triggers a hard delete, meaning that once executed, the user data cannot be recovered.
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpDelete("deleteProfile")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteOwnProfile()
    {
        await _mediator.Send(new DeleteProfileCommand(UserId));
        return Ok(ApiResponse<string>.SuccessResponse("Profile and all associated data deleted permanently."));
    }
}
