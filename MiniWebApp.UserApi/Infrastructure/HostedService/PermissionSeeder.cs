using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Infrastructure.HostedService;

public class PermissionSeeder(UserDbContext dbContext) : IPermissionSeeder
{
    public async Task SeedAsync(CancellationToken ct)
    {
        // 1. Extract permissions from your static constants
        var permissionData = GetPermissionsToSeed();

        // 2. Fetch existing permissions to avoid duplicates
        var existingCodesSet = await dbContext.Permissions
            .Select(p => p.Code)
            .ToHashSetAsync(ct);

        var newPermissions = permissionData
            .Where(p => !existingCodesSet.Contains(p.Code))
            .ToList();

        if (newPermissions.Count != 0)
        {
            await dbContext.Permissions.AddRangeAsync(newPermissions, ct);
            await dbContext.SaveChangesAsync(ct);
        }
    }
    
    private static List<Permission> GetPermissionsToSeed()
    {
        var list = new List<Permission>();

        list.AddRange(BuildPermissions(AppPermissions.Tenants.All, nameof(AppPermissions.Tenants)));
        list.AddRange(BuildPermissions(AppPermissions.Roles.All, nameof(AppPermissions.Roles)));
        list.AddRange(BuildPermissions(AppPermissions.Users.All, nameof(AppPermissions.Users)));
        list.AddRange(BuildPermissions(AppPermissions.Permissions.All, nameof(AppPermissions.Permissions)));

        return list;
    }

    private static IEnumerable<Permission> BuildPermissions(IEnumerable<string> codes, string category)
    {
        return codes.Select(code => new Permission
        {
            Id = Guid.NewGuid(),
            Code = code,
            Category = category,
            Description = $"Allows {code.Replace('.', ' ')}"
        });
    }
}
