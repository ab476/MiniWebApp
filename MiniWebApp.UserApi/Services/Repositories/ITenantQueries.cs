using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Models.Tenants;

namespace MiniWebApp.UserApi.Services.Repositories;

public interface ITenantQueries
{
    Task<HashSet<string>> GetExistingTenantNamesAsync(CancellationToken ct = default);
    Task<Outcome<TenantResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Outcome<IReadOnlyList<TenantResponse>>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
}