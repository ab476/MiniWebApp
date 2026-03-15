using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Repositories;

public sealed class TenantRepository(UserDbContext dbContext, IRequestContext requestContext)
    : RepositoryBase(requestContext), ITenantRepository
{
    public async Task<Outcome<Tenant[]>> CreateBulkAsync(IEnumerable<Tenant> tenants, CancellationToken ct = default)
    {
        var tenantList = tenants.ToList();
        if (tenantList.Count == 0)
        {
            return (StatusCodes.Status200OK, Array.Empty<Tenant>());
        }

        await dbContext.Tenants.AddRangeAsync(tenantList, ct);
        await dbContext.SaveChangesAsync(ct);
        return (StatusCodes.Status201Created, tenantList.ToArray());
    }
    public async Task<Outcome<TenantResponse>> CreateAsync(
        CreateTenantRequest request,
        CancellationToken ct = default)
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Domain = request.Domain,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Tenants.Add(tenant);
        await dbContext.SaveChangesAsync(ct);

        return (StatusCodes.Status201Created, tenant.ToResponse());
    }

    public async Task<Outcome> UpdateAsync(
        Guid tenantId,
        UpdateTenantRequest request,
        CancellationToken ct = default)
    {
        var rowsAffected = await dbContext.Tenants
            .TagWith("Tenants.Update")
            .Where(t => t.Id == tenantId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.Name, request.Name)
                .SetProperty(t => t.Domain, request.Domain)
                .SetProperty(t => t.UpdatedAt, DateTime.UtcNow), ct);

        return rowsAffected == 1
            ? StatusCodes.Status200OK
            : ("Tenant not found.", StatusCodes.Status404NotFound);
    }

    public async Task<Outcome> ActivateAsync(
        ActivateTenantRequest request,
        CancellationToken ct = default)
    {
        var rowsAffected = await dbContext.Tenants
            .TagWith("Tenants.Activate")
            .Where(t => t.Id == request.TenantId && !t.IsActive)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.IsActive, true)
                .SetProperty(t => t.UpdatedAt, DateTime.UtcNow), ct);

        return rowsAffected == 1
            ? StatusCodes.Status200OK
            : ("Tenant not found or already active.", StatusCodes.Status404NotFound);
    }

    public async Task<Outcome> DeactivateAsync(
        DeactivateTenantRequest request,
        CancellationToken ct = default)
    {
        var rowsAffected = await dbContext.Tenants
            .TagWith("Tenants.Deactivate")
            .Where(t => t.Id == request.TenantId && t.IsActive)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.IsActive, false)
                .SetProperty(t => t.UpdatedAt, DateTime.UtcNow), ct);

        return rowsAffected == 1
            ? StatusCodes.Status200OK
            : ("Tenant not found or already inactive.", StatusCodes.Status404NotFound);
    }

    public async Task<Outcome> DeleteAsync(Guid tenantId, CancellationToken ct = default)
    {
        var rowsAffected = await dbContext.Tenants
            .TagWith("Tenants.Delete")
            .Where(t => t.Id == tenantId)
            .ExecuteDeleteAsync(ct);

        return rowsAffected == 1
            ? StatusCodes.Status200OK
            : ("Tenant not found.", StatusCodes.Status404NotFound);
    }
}