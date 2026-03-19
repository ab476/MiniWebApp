namespace MiniWebApp.UserApi.Services.Repositories;

public interface ITenantQueries
{
    Task<Outcome<HashSet<string>>> GetExistingTenantNamesAsync(CancellationToken ct = default);
    Task<Outcome<TenantResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Outcome<List<TenantResponse>>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
}