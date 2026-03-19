using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Repositories;

public sealed class RoleClaimQueries(UserDbContext db, IRequestContext requestContext)
    : RepositoryBase(requestContext), IRoleClaimQueries
{
    public async Task<Outcome<ClaimResponse[]>> GetClaimsByRoleAsync(
        string roleCode,
        CancellationToken ct = default)
    {
        var claims = await db.RoleClaims
            .TagWith("RoleClaimQueries.GetClaimsByRoleAsync")
            .AsNoTracking()
            .Where(rc => rc.TenantId == ContextTenantId && rc.RoleCode == roleCode)
            .Select(rc => rc.Claim)
            .ProjectToResponse()
            .ToArrayAsync(ct);

        return Outcome.Success(StatusCodes.Status200OK, claims);
    }

    public async Task<Outcome<ClaimResponse[]>> GetClaimsByRolesAsync(
        string[] roleCodes,
        CancellationToken ct = default)
    {
        if (roleCodes is null || roleCodes.Length == 0)
        {
            return Outcome.Success(StatusCodes.Status200OK, Array.Empty<ClaimResponse>());
        }

        var claims = await db.RoleClaims
            .TagWith("RoleClaimQueries.GetClaimsByRolesAsync")
            .AsNoTracking()
            .Where(rc => rc.TenantId == ContextTenantId && roleCodes.Contains(rc.RoleCode))
            .Select(rc => rc.Claim)
            .Distinct()
            .ProjectToResponse()
            .ToArrayAsync(ct);

        return Outcome.Success(StatusCodes.Status200OK, claims);
    }

    public async Task<Outcome<bool>> HasClaimAsync(
        RoleClaimDto request,
        CancellationToken ct = default)
    {
        // Querying the junction table directly using the TenantId from the provider
        var hasAccess = await db.RoleClaims
            .TagWith("RoleClaimQueries.HasClaimAsync")
            .AsNoTracking()
            .AnyAsync(rc =>
                rc.TenantId == ContextTenantId &&
                rc.RoleCode == request.RoleCode &&
                rc.ClaimCode == request.ClaimCode, ct);

        return Outcome.Success(StatusCodes.Status200OK, hasAccess);
    }
}