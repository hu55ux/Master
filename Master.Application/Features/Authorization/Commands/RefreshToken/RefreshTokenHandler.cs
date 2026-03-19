using System.Security.Claims;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Application.Services;
using MediatR;

namespace Master.Application.Features.Authorization.Commands.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDTO>
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenHandler(IAuthRepository authRepository, ITokenService tokenService)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDTO> Handle(RefreshTokenCommand command, CancellationToken ct)
    {
        var (principal, jti) = _tokenService.ValidateRefreshJwtAndGetJti(command.Request.RefreshToken);

        var storedToken = await _authRepository.GetRefreshTokenByJtiAsync(jti);

        if (storedToken is null || !storedToken.IsActive)
            throw new UnauthorizedAccessException("Invalid token.");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _authRepository.GetByIdAsync(userId!);

        if (user is null) throw new UnauthorizedAccessException("User not found.");

        storedToken.RevokedAt = DateTimeOffset.UtcNow;

        var newTokens = await _tokenService.GenerateTokensAsync(user);

        var newJti = _tokenService.GetJtiFromRefreshToken(newTokens.RefreshToken);
        storedToken.ReplacedByJwtId = newJti;

        _authRepository.UpdateRefreshToken(storedToken);
        await _authRepository.SaveChangesAsync(ct);

        return newTokens;
    }
}