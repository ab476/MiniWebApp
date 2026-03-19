namespace MiniWebApp.UserApi.Services.Repositories;

public interface IUserRoleRepository
{
    Task<Outcome> CreateBulkAsync(IEnumerable<UserRole> userRoles, CancellationToken ct = default);
    Task<Outcome> AssignAsync(UserRoleRequest request, CancellationToken ct = default);
    Task<Outcome> AssignBulkAsync(BulkUserRoleRequest request, CancellationToken ct = default);
    Task<Outcome> RevokeAsync(UserRoleRequest request, CancellationToken ct = default);
    Task<Outcome> RevokeAllForUserAsync(Guid userId, CancellationToken ct = default);
}