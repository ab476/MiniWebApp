namespace MiniWebApp.UserApi.Services.Repositories;

public interface IRoleQueries
{
    
    Task<Outcome<RoleResponse>> GetByRoleCodeAsync(string roleCode, CancellationToken ct = default);
    Task<Outcome<List<RoleResponse>>> GetPagedAsync(Guid tenantId, int page, int pageSize, CancellationToken ct = default);
    /// <summary>
    /// Fetches all existing role codes for a specific tenant.
    /// </summary>
    Task<Outcome<HashSet<string>>> GetExistingRoleCodesAsync(Guid tenantId, CancellationToken ct = default);
}
