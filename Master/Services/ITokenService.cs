using System.Security.Claims;
using Master.DTOs;
using Master.Models;

namespace Master.Services;
public interface ITokenService
{
    Task<AuthResponseDTO> GenerateTokensAsync(AppUser user);
    (ClaimsPrincipal principal, string jti) ValidateRefreshJwtAndGetJti(string refreshToken, bool validateLifetime = true);
    string GetJtiFromRefreshToken(string refreshJwt);
}
