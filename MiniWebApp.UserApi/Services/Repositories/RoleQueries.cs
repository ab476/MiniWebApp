using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Repositories;

public sealed class RoleQueries(UserDbContext db, IRequestContext requestContext)
    : RepositoryBase(requestContext), IRoleQueries
{
    public async Task<Outcome<RoleResponse>> GetByRoleCodeAsync(string roleCode, CancellationToken ct = default)
    {
        var result = await db.Roles
            .AsNoTracking()
            .Where(r => r.TenantId == ContextTenantId && r.RoleCode == roleCode)
            .ProjectToResponse() // Using projection to avoid loading the whole entity
            .FirstOrDefaultAsync(ct);

        return result is null
            ? Outcome.Failure("Role not found.", StatusCodes.Status404NotFound)
            : Outcome.Success(StatusCodes.Status200OK, result);
    }
    /// <inheritdoc />
    public async Task<Outcome<HashSet<string>>> GetExistingRoleCodesAsync(Guid tenantId, CancellationToken ct = default)
    {
        var roleCodes = await db.Roles
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId)
            .Select(r => r.RoleCode)
            .ToHashSetAsync(StringComparer.InvariantCultureIgnoreCase, ct);

        return Outcome.Success(roleCodes);
    }
    public async Task<Outcome<List<RoleResponse>>> GetPagedAsync(
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

        return Outcome.Success(StatusCodes.Status200OK, roles);
    }
}
