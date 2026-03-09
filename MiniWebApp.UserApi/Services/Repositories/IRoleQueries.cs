using MiniWebApp.UserApi.Models.Roles;

namespace MiniWebApp.UserApi.Services.Repositories;

public interface IRoleQueries
{
    
    Task<Outcome<RoleResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Outcome<IReadOnlyList<RoleResponse>>> GetPagedAsync(Guid tenantId, int page, int pageSize, CancellationToken ct = default);
}
