using MiniWebApp.Core.Common;
using MiniWebApp.UserApi.Contracts.Roles;

namespace MiniWebApp.UserApi.Application;

public interface IRoleService
{
    Task<Outcome<RoleResponse>> CreateAsync(CreateRoleRequest request, CancellationToken ct = default);
    Task<Outcome> DeleteAsync(Guid roleId, CancellationToken ct = default);
    Task<Outcome<RoleResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Outcome<IReadOnlyList<RoleResponse>>> GetPagedAsync(Guid tenantId, int page, int pageSize, CancellationToken ct = default);
    Task<Outcome> UpdateAsync(Guid roleId, UpdateRoleRequest request, CancellationToken ct = default);
}