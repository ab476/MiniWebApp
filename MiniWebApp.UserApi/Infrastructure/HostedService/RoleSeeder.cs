using Microsoft.Extensions.Options;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Options;

namespace MiniWebApp.UserApi.Infrastructure.HostedService;

public class RoleSeeder(
    UserDbContext dbContext,
    IOptions<SeedDataOptions> options) : SeederBase(dbContext), IRoleSeeder
{
    private readonly SeedDataOptions _seedData = options.Value;

    public async Task SeedAsync(CancellationToken ct)
    {
        if (_seedData.Roles.Count == 0) return;

        var existingRoleNames = await _dbContext.Set<TRole>()
            .Select(r => r.Name)
            .ToHashSetAsync(StringComparer.InvariantCultureIgnoreCase, ct);

        var permissionMap = await _dbContext.Set<TPermission>()
            .ToDictionaryAsync(p => p.Code, p => p.Id, StringComparer.InvariantCultureIgnoreCase, ct);
        var tennantId = await GetDefaultTenantIdAsync(ct);
        foreach (var roleSeed in _seedData.Roles)
        {
            if (existingRoleNames.Contains(roleSeed.Name)) continue;

            var newRole = new TRole
            {
                Id = Guid.NewGuid(),
                TenantId = tennantId,
                Name = roleSeed.Name,
                NormalizedName = roleSeed.Name.ToUpperInvariant()
            };

            // logic: If IncludeAll is true, take every ID in the map. 
            // Otherwise, filter by the list provided in JSON.
            IEnumerable<Guid> permissionIdsToAssign = roleSeed.IncludeAll
                ? permissionMap.Values
                : roleSeed.Permissions
                    .Where(permissionMap.ContainsKey)
                    .Select(pCode => permissionMap[pCode]);

            var rolePermissions = permissionIdsToAssign
                .Select(pId => new TRolePermission
                {
                    RoleId = newRole.Id,
                    PermissionId = pId
                })
                .ToList();

            await _dbContext.Set<TRole>().AddAsync(newRole, ct);

            if (rolePermissions.Count != 0)
            {
                await _dbContext.Set<TRolePermission>().AddRangeAsync(rolePermissions, ct);
            }
        }

        await _dbContext.SaveChangesAsync(ct);
    }


}
