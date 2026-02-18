using MiniWebApp.Core.Common;
using MiniWebApp.UserApi.Models.Tenants;

namespace MiniWebApp.UserApi.Services.Tenants;

public interface ITenantService
{
    Task<Outcome> ActivateAsync(ActivateTenantRequest request, CancellationToken ct = default);
    Task<Outcome<TenantResponse>> CreateAsync(CreateTenantRequest request, CancellationToken ct = default);
    Task<Outcome> DeactivateAsync(DeactivateTenantRequest request, CancellationToken ct = default);
    Task<Outcome> DeleteAsync(Guid tenantId, CancellationToken ct = default);
    Task<Outcome<TenantResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Outcome<IReadOnlyList<TenantResponse>>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Outcome> UpdateAsync(Guid tenantId, UpdateTenantRequest request, CancellationToken ct = default);
}