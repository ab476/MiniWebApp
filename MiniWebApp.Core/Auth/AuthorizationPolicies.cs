namespace MiniWebApp.Core.Auth;

public static class AppRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string User = "User";

    public static IReadOnlyCollection<string> All =>
    [
        SuperAdmin,
        Admin,
        Manager,
        User
    ];
}

public static class AppPermissions
{
    public static class Tenants
    {
        public const string Read = "tenants.read";
        public const string Write = "tenants.write";
        public const string Manage = "tenants.manage";

        public static IReadOnlyCollection<string> All =>
            [Read, Write, Manage];
    }

    public static class Roles
    {
        public const string Read = "roles.read";
        public const string Write = "roles.write";
        public const string Manage = "roles.manage";

        public static IReadOnlyCollection<string> All =>
            [Read, Write, Manage];
    }

    public static class Users
    {
        public const string Read = "users.read";
        public const string Write = "users.write";
        public const string Manage = "users.manage";

        public static IReadOnlyCollection<string> All =>
            [Read, Write, Manage];
    }

    public static IReadOnlyCollection<string> All =>
        [.. Tenants.All , .. Roles.All, .. Users.All];
}

public static class AppClaimTypes
{

    public const string Permissions = "miniwebapp/permissions";
    public const string TenantId = "miniwebapp/tenant_id";
}