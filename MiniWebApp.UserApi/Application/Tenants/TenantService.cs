using Microsoft.EntityFrameworkCore;
using MiniWebApp.Core.Common;
using MiniWebApp.UserApi.Contracts.Tenants;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Domain.Models;

namespace MiniWebApp.UserApi.Application.Tenants;

public class TenantService(UserDbContext _db) : ITenantService
{
    public async Task<Outcome<TenantResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var tenant = await _db.Tenants
            .TagWith($"{nameof(TenantService)}.{nameof(GetByIdAsync)}")
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        return tenant is null
            ? ("Tenant not found.", StatusCodes.Status404NotFound)
            : (StatusCodes.Status200OK, tenant.ToResponse());
    }

    public async Task<Outcome<IReadOnlyList<TenantResponse>>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var tenants = await _db.Tenants
            .TagWith($"{nameof(TenantService)}.{nameof(GetPagedAsync)}")
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToResponse()
            .ToListAsync(ct);

        return (StatusCodes.Status200OK, tenants);
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

        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync(ct);

        return (StatusCodes.Status201Created, tenant.ToResponse());
    }

    public async Task<Outcome> UpdateAsync(
        Guid tenantId,
        UpdateTenantRequest request,
        CancellationToken ct = default)
    {
        var rowsAffected = await _db.Tenants
            .TagWith($"{nameof(TenantService)}.{nameof(UpdateAsync)}")
            .Where(t => t.Id == tenantId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.Name, request.Name)
                .SetProperty(t => t.Domain, request.Domain)
                .SetProperty(t => t.UpdatedAt, DateTime.UtcNow), ct);

        return rowsAffected == 1
            ? StatusCodes.Status200OK
            : (StatusCodes.Status404NotFound, "Tenant not found.");
    }

    public async Task<Outcome> ActivateAsync(
        ActivateTenantRequest request,
        CancellationToken ct = default)
    {
        var rowsAffected = await _db.Tenants
            .TagWith($"{nameof(TenantService)}.{nameof(ActivateAsync)}")
            .Where(t => t.Id == request.TenantId && !t.IsActive)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.IsActive, true)
                .SetProperty(t => t.UpdatedAt, DateTime.UtcNow), ct);

        return rowsAffected == 1
            ? StatusCodes.Status200OK
            : (StatusCodes.Status404NotFound, "Tenant not found or already active.");
    }

    public async Task<Outcome> DeactivateAsync(
        DeactivateTenantRequest request,
        CancellationToken ct = default)
    {
        var rowsAffected = await _db.Tenants
            .TagWith($"{nameof(TenantService)}.{nameof(DeactivateAsync)}")
            .Where(t => t.Id == request.TenantId && t.IsActive)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.IsActive, false)
                .SetProperty(t => t.UpdatedAt, DateTime.UtcNow), ct);

        return rowsAffected == 1
            ? StatusCodes.Status200OK
            : (StatusCodes.Status404NotFound, "Tenant not found or already inactive.");
    }

    public async Task<Outcome> DeleteAsync(Guid tenantId, CancellationToken ct = default)
    {
        var rowsAffected = await _db.Tenants
            .TagWith($"{nameof(TenantService)}.{nameof(DeleteAsync)}")
            .Where(t => t.Id == tenantId)
            .ExecuteDeleteAsync(ct);

        return rowsAffected == 1
            ? StatusCodes.Status200OK
            : (StatusCodes.Status404NotFound, "Tenant not found.");
    }
}
