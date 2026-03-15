namespace MiniWebApp.UserApi.Services.Repositories;

public interface ITenantRepository
{
    Task<Outcome<Tenant[]>> CreateBulkAsync(IEnumerable<Tenant> tenants, CancellationToken ct = default);
    Task<Outcome<TenantResponse>> CreateAsync(CreateTenantRequest request, CancellationToken ct = default);
    Task<Outcome> ActivateAsync(ActivateTenantRequest request, CancellationToken ct = default);
    Task<Outcome> DeactivateAsync(DeactivateTenantRequest request, CancellationToken ct = default);
    Task<Outcome> DeleteAsync(Guid tenantId, CancellationToken ct = default);
    Task<Outcome> UpdateAsync(Guid tenantId, UpdateTenantRequest request, CancellationToken ct = default);
}