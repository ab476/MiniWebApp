namespace MiniWebApp.UserApi.Services.Repositories;

public interface IRoleClaimRepository
{
    Task<Outcome> AssignAsync(RoleClaimDto request, CancellationToken ct = default);
    Task<Outcome> AssignBulkAsync(BulkRoleClaimRequest request, CancellationToken ct = default);
    Task<Outcome> RevokeAllForRoleAsync(string roleCode, CancellationToken ct = default);
    Task<Outcome> RevokeAsync(RoleClaimDto request, CancellationToken ct = default);
}