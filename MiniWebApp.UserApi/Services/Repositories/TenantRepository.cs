using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Infrastructure;

namespace MiniWebApp.UserApi.Services.Repositories;

public sealed class TenantRepository(
    UserDbContext dbContext,
    IRequestContext requestContext,
    IAuditChannel auditChannel)
    : RepositoryBase(requestContext), ITenantRepository
{
    public async Task<Outcome<Tenant[]>> CreateBulkAsync(IEnumerable<CreateTenantRequest> tenants, CancellationToken ct = default)
    {
        var timestamp = UtcNow;
        var tenantList = tenants.Select(x => x.ToEntity(timestamp)).ToList();
        if (tenantList.Count == 0)
        {
            return (StatusCodes.Status200OK, Array.Empty<Tenant>());
        }

        await dbContext.Tenants.AddRangeAsync(tenantList, ct);
        await dbContext.SaveChangesAsync(ct);

        await auditChannel.PublishAsync(tenantList.ToAuditRequest(AuditAction.Create, UserId, timestamp), ct);

        return Outcome.Success(StatusCodes.Status201Created, tenantList.ToArray());
    }
    public async Task<Outcome<TenantResponse>> CreateAsync(
        CreateTenantRequest request,
        CancellationToken ct = default)
    {
        var tenant = request.ToEntity(timeStamp: UtcNow);

        dbContext.Tenants.Add(tenant);
        await dbContext.SaveChangesAsync(ct);

        await auditChannel.PublishAsync(tenant.ToAuditRequest(AuditAction.Create, UserId), ct);

        return Outcome.Success(StatusCodes.Status201Created, tenant.ToResponse());
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
            ? Outcome.Success(StatusCodes.Status200OK)
            : Outcome.Failure("Tenant not found.", StatusCodes.Status404NotFound);
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
            ? Outcome.Success(StatusCodes.Status200OK)
            : Outcome.Failure("Tenant not found or already active.", StatusCodes.Status404NotFound);
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
            ? Outcome.Success(StatusCodes.Status200OK)
            : Outcome.Failure("Tenant not found or already inactive.", StatusCodes.Status404NotFound);
    }

    public async Task<Outcome> DeleteAsync(Guid tenantId, CancellationToken ct = default)
    {
        var rowsAffected = await dbContext.Tenants
            .TagWith("Tenants.Delete")
            .Where(t => t.Id == tenantId)
            .ExecuteDeleteAsync(ct);

        return rowsAffected == 1
            ? Outcome.Success(StatusCodes.Status200OK)
            : Outcome.Failure("Tenant not found.", StatusCodes.Status404NotFound);
    }
}