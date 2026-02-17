namespace MiniWebApp.UserApi.Options;

public sealed class SeedDataOptions
{
    public List<TenantSeed> Tenants { get; init; } = [];
    public List<RoleSeed> Roles { get; init; } = [];
    public List<UserSeed> Users { get; init; } = [];
}

public sealed class TenantSeed
{
    public string Name { get; init; } = default!;
    public string? Domain { get; init; }
}

public sealed class RoleSeed
{
    public string Name { get; init; } = default!;
    public List<string> Permissions { get; init; } = [];
}

public sealed class UserSeed
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public List<string> Roles { get; init; } = [];
}
