namespace Master.Domain.Constants
{
    /// <summary>
    /// Defines application roles used for authentication and authorization.
    /// </summary>
    public static class UserRoles
    {
        /// <summary>
        /// Administrator role.
        /// </summary>
        public const string Admin = "Admin";

        /// <summary>
        /// Standard user role.
        /// </summary>
        public const string User = "User";

        /// <summary>
        /// Master (service provider) role.
        /// </summary>
        public const string Master = "Master";

        /// <summary>
        /// Client (job requester) role.
        /// </summary>
        public const string Client = "Client";
    }

    /// <summary>
    /// Defines authorization policies for controlling access based on roles.
    /// </summary>
    public static class AuthPolicies
    {
        /// <summary>
        /// Policy that allows only administrators.
        /// </summary>
        public const string AdminOnly = "AdminOnly";

        /// <summary>
        /// Policy that allows only masters (service providers).
        /// </summary>
        public const string MasterOnly = "MasterOnly";

        /// <summary>
        /// Policy that allows only clients (job requesters).
        /// </summary>
        public const string ClientOnly = "ClientOnly";

        /// <summary>
        /// Policy that allows only masters and admin
        /// </summary>
        public const string MasterOrAdmin = "MasterOrAdmin";

        /// <summary>
        /// Policy that allows only clients or admin
        /// </summary>
        public const string ClientOrAdmin = "ClientOrAdmin";
    }
}
