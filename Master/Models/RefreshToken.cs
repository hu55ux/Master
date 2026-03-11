namespace Master.Models;

/// <summary>
/// Represents a refresh token used for generating new access tokens in JWT authentication.
/// </summary>
/// <remarks>
/// Refresh tokens allow clients to obtain new access tokens without requiring the user
/// to log in again. Each refresh token is linked to a specific JWT and user.
/// 
/// Token lifecycle:
/// - Created when a user logs in
/// - Used to request a new access token
/// - Can be revoked or replaced during token rotation
/// </remarks>
public class RefreshToken
{
    /// <summary>
    /// Gets or sets the unique identifier of the refresh token.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the JWT access token associated with this refresh token.
    /// </summary>
    public string JwtId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the user who owns this refresh token.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration time of the refresh token.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the time when the refresh token was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the time when the refresh token was revoked.
    /// </summary>
    /// <remarks>
    /// If this value is set, the token is considered revoked and cannot be used anymore.
    /// </remarks>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>
    /// Gets or sets the JWT ID of the new token that replaced this one during token rotation.
    /// </summary>
    public string? ReplacedByJwtId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the refresh token has been revoked.
    /// </summary>
    public bool IsRevoked => RevokedAt.HasValue;

    /// <summary>
    /// Gets a value indicating whether the refresh token has expired.
    /// </summary>
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

    /// <summary>
    /// Gets a value indicating whether the refresh token is active.
    /// </summary>
    /// <remarks>
    /// A token is active if it is neither revoked nor expired.
    /// </remarks>
    public bool IsActive => !IsRevoked && !IsExpired;
}