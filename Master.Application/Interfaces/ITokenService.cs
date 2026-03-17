using System.Security.Claims;
using Master.Application.DTOs;
using Master.Application.Models;

namespace Master.Application.Services;
public interface ITokenService
{
    Task<AuthResponseDTO> GenerateTokensAsync(AppUser user);
    (ClaimsPrincipal principal, string jti) ValidateRefreshJwtAndGetJti(string refreshToken, bool validateLifetime = true);
    string GetJtiFromRefreshToken(string refreshJwt);
}
