namespace MiniWebApp.UserApi.Services.Repositories;

public interface IRoleClaimQueries
{
    Task<Outcome<ClaimResponse[]>> GetClaimsByRoleAsync(string roleCode, CancellationToken ct = default);
    Task<Outcome<ClaimResponse[]>> GetClaimsByRolesAsync(string[] roleCodes, CancellationToken ct = default);
    Task<Outcome<bool>> HasClaimAsync(RoleClaimDto request, CancellationToken ct = default);
}