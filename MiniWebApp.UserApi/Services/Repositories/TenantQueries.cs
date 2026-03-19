using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Repositories;

public sealed class TenantQueries(UserDbContext dbContext, IRequestContext requestContext)
    : RepositoryBase(requestContext), ITenantQueries
{
    public async Task<Outcome<HashSet<string>>> GetExistingTenantNamesAsync(CancellationToken ct = default)
    {
        var tenantNames = await dbContext.Tenants
            .AsNoTracking()
            .Select(t => t.Name)
            .ToHashSetAsync(StringComparer.InvariantCultureIgnoreCase, ct);

        return Outcome.Success(tenantNames);
    }

    public async Task<Outcome<TenantResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var tenant = await dbContext.Tenants
            .TagWith("Tenants.GetById")
            .AsNoTracking()
            .ProjectToResponse()
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        return tenant is null
            ? Outcome.Failure("Tenant not found.", StatusCodes.Status404NotFound)
            : Outcome.Success(StatusCodes.Status200OK, tenant);
    }

    public async Task<Outcome<List<TenantResponse>>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var tenants = await dbContext.Tenants.TagWith("Tenants.GetPaged").AsNoTracking().OrderByDescending(t => t.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ProjectToResponse().ToListAsync(ct);

        return Outcome.Success(StatusCodes.Status200OK, tenants);
    }
}