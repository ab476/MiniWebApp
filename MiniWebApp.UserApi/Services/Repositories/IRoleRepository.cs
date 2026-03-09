using MiniWebApp.UserApi.Models.Roles;

namespace MiniWebApp.UserApi.Services.Repositories;

public interface IRoleRepository
{
    Task<Outcome<RoleResponse>> CreateAsync(CreateRoleRequest request, CancellationToken ct = default);
    Task<Outcome> DeleteAsync(Guid roleId, CancellationToken ct = default);
    Task<Outcome> UpdateAsync(Guid roleId, UpdateRoleRequest request, CancellationToken ct = default);
}