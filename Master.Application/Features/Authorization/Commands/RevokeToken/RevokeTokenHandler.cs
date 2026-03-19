using Master.Application.Interfaces;
using Master.Application.Services;
using MediatR;

namespace Master.Application.Features.Authorization.Commands.RevokeToken;

public class RevokeTokenHandler : IRequestHandler<RevokeTokenCommand, Unit>
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;

    public RevokeTokenHandler(IAuthRepository authRepository, ITokenService tokenService)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
    }

    public async Task<Unit> Handle(RevokeTokenCommand command, CancellationToken ct)
    {
        string? jti;
        try
        {
            (_, jti) = _tokenService.ValidateRefreshJwtAndGetJti(command.RefreshToken, validateLifetime: false);
        }
        catch
        {
            return Unit.Value;
        }

        var storedToken = await _authRepository.GetRefreshTokenByJtiAsync(jti!);

        if (storedToken != null && storedToken.IsActive)
        {
            storedToken.RevokedAt = DateTimeOffset.UtcNow;

            _authRepository.UpdateRefreshToken(storedToken);
            await _authRepository.SaveChangesAsync(ct);
        }

        return Unit.Value;
    }
}