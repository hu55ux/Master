namespace Master.Infrastructure.Config
{
    /// <summary>
    /// Represents JWT configuration settings.
    /// These values are usually bound from appsettings.json under the "JwtSettings" section.
    /// </summary>
    public class JwtConfig
    {
        /// <summary>
        /// The configuration section name in appsettings.json.
        /// </summary>
        public const string SectionName = "JWTSettings";

        /// <summary>
        /// Secret key used to sign JWT tokens.
        /// Should be long and secure to prevent token tampering.
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Secret key used specifically for signing refresh tokens.
        /// Can be different from <see cref="SecretKey"/>.
        /// </summary>
        public string RefreshTokenSecretKey { get; set; } = string.Empty;

        /// <summary>
        /// JWT issuer. Typically the URL or identifier of the issuing authority.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// JWT audience. Usually identifies the intended recipients of the token.
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Expiration time in minutes for the access token.
        /// Default is 150 minutes.
        /// </summary>
        public int ExpirationMinutes { get; set; } = 150;

        /// <summary>
        /// Expiration time in days for refresh tokens.
        /// Default is 7 days.
        /// </summary>
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}