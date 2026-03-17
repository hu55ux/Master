using Master.Data;
using Master.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Features.Authorization.Commands.RevokeToken;

public class RevokeTokenHandler : IRequestHandler<RevokeTokenCommand, Unit>
{
    private readonly MasterDbContext _context;
    private readonly ITokenService _tokenService;

    public RevokeTokenHandler(MasterDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<Unit> Handle(RevokeTokenCommand command, CancellationToken ct)
    {
        string? jti;
        try
        {
            (_, jti) = _tokenService.ValidateRefreshJwtAndGetJti(command.RefreshToken, validateLifetime: false);
        }
        catch { return Unit.Value; }

        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jti, ct);
        if (storedToken != null && storedToken.IsActive)
        {
            storedToken.RevokedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync(ct);
        }

        return Unit.Value;
    }
}
