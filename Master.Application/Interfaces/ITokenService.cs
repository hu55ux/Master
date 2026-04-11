using System.Security.Claims;
using Master.Application.DTOs;
using Master.Domain.Models;

namespace Master.Application.Interfaces;

/// <summary>
/// Interface for service operations related to security tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a set of access and refresh tokens for a specific user.
    /// </summary>
    Task<AuthResponseDTO> GenerateTokensAsync(AppUser user);

    /// <summary>
    /// Validates a refresh token and extracts the JWT identifier (JTI).
    /// </summary>
    (ClaimsPrincipal principal, string jti) ValidateRefreshJwtAndGetJti(string refreshToken, bool validateLifetime = true);

    /// <summary>
    /// Extracts the JWT identifier (JTI) from a refresh token without full validation.
    /// </summary>
    string GetJtiFromRefreshToken(string refreshJwt);
}
