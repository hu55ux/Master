using System.Security.Claims;
using Master.Application.DTOs;
using Master.Application.Models;
using Master.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Master.Application.Features.Authorization.Commands.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDTO>
{
    private readonly MasterDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public RefreshTokenHandler(MasterDbContext context, UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _context = context;
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDTO> Handle(RefreshTokenCommand command, CancellationToken ct)
    {
        var (principal, jti) = _tokenService.ValidateRefreshJwtAndGetJti(command.Request.RefreshToken);
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jti);

        if (storedToken is null || !storedToken.IsActive) throw new UnauthorizedAccessException("Invalid token.");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user is null) throw new UnauthorizedAccessException("User not found.");

        storedToken.RevokedAt = DateTimeOffset.UtcNow;
        var newTokens = await _tokenService.GenerateTokensAsync(user);

        var newJti = _tokenService.GetJtiFromRefreshToken(newTokens.RefreshToken);
        storedToken.ReplacedByJwtId = newJti;

        await _context.SaveChangesAsync();
        return newTokens;
    }
}