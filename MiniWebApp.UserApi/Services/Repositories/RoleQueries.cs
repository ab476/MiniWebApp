using MiniWebApp.UserApi.Contracts.Roles;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Models.Roles;

namespace MiniWebApp.UserApi.Services.Repositories;

public sealed class RoleQueries(UserDbContext db) : IRoleQueries
{
    public async Task<Outcome<RoleResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var result = await db.Roles
            .TagWith("RoleQueries.GetByIdAsync: Fetching role by primary key")
            .AsNoTracking()
            .ProjectToResponse() // Using projection to avoid loading the whole entity
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        return result is null
            ? ("Role not found.", StatusCodes.Status404NotFound)
            : (StatusCodes.Status200OK, result);
    }

    public async Task<Outcome<IReadOnlyList<RoleResponse>>> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var roles = await db.Roles
            .TagWith("RoleQueries.GetPagedAsync: Fetching paged roles for tenant")
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToResponse()
            .ToListAsync(ct);

        return (StatusCodes.Status200OK, roles);
    }
}
