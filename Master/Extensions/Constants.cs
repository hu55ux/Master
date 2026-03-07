namespace Master.Extensions;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Master = "Master";
    public const string Client = "Client";
}

public static class AuthPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string MasterOnly = "MasterOnly";
    public const string ClientOnly = "ClientOnly";
}
