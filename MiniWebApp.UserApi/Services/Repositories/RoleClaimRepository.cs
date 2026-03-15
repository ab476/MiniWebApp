using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Repositories;

public sealed record RoleClaimDto(string RoleCode, string ClaimCode);

public sealed record BulkRoleClaimRequest(string RoleCode, Guid TenantId, string[] ClaimCodes);

public sealed class RoleClaimRepository(UserDbContext db, IRequestContext requestContext)
    : RepositoryBase(requestContext), IRoleClaimRepository
{
    public async Task<Outcome> AssignAsync(
        RoleClaimDto request,
        CancellationToken ct = default)
    {
        // Check existence within the specific tenant context
        var exists = await db.RoleClaims
            .AnyAsync(rc => rc.TenantId == ContextTenantId &&
                            rc.RoleCode == request.RoleCode &&
                            rc.ClaimCode == request.ClaimCode, ct);

        if (exists) return ("Claim already assigned to this role.", StatusCodes.Status409Conflict);

        var assignment = new RoleClaim
        {
            TenantId = ContextTenantId,
            RoleCode = request.RoleCode,
            ClaimCode = request.ClaimCode
        };

        await db.RoleClaims.AddAsync(assignment, ct);
        await db.SaveChangesAsync(ct);

        return StatusCodes.Status201Created;
    }

    public async Task<Outcome> AssignBulkAsync(
        BulkRoleClaimRequest request,
        CancellationToken ct = default)
    {
        if (request.TenantId != ContextTenantId && !IsSuperAdmin)
        {
            return ("Access Denied: You do not have permission to manage role claims for another tenant.",
                    StatusCodes.Status403Forbidden);
        }

        if (request.TenantId == Guid.Empty)
        {
            return ("TenantId must be provided for bulk role claim assignments.", StatusCodes.Status400BadRequest);
        }


        // 1. Get existing claims for this role in this tenant
        var existingClaimCodes = await db.RoleClaims
            .AsNoTracking()
            .Where(rc => rc.TenantId == request.TenantId && rc.RoleCode == request.RoleCode)
            .Select(rc => rc.ClaimCode)
            .ToListAsync(ct);

        // 2. Filter for new assignments
        var newAssignments = request.ClaimCodes
            .Distinct()
            .Except(existingClaimCodes)
            .Select(code => new RoleClaim
            {
                TenantId = request.TenantId,
                RoleCode = request.RoleCode,
                ClaimCode = code
            })
            .ToList();

        if (newAssignments.Count == 0)
        {
            return ("No new claims to assign.", StatusCodes.Status200OK);
        }

        await db.RoleClaims.AddRangeAsync(newAssignments, ct);
        await db.SaveChangesAsync(ct);

        return StatusCodes.Status201Created;
    }

    public async Task<Outcome> RevokeAsync(
        RoleClaimDto request,
        CancellationToken ct = default)
    {
        var rows = await db.RoleClaims
            .Where(rc => rc.TenantId == ContextTenantId &&
                         rc.RoleCode == request.RoleCode &&
                         rc.ClaimCode == request.ClaimCode)
            .ExecuteDeleteAsync(ct);

        return rows > 0
            ? StatusCodes.Status200OK
            : ("Assignment not found.", StatusCodes.Status404NotFound);
    }

    public async Task<Outcome> RevokeAllForRoleAsync(string roleCode, CancellationToken ct = default)
    {
        await db.RoleClaims
            .Where(rc => rc.TenantId == ContextTenantId && rc.RoleCode == roleCode)
            .ExecuteDeleteAsync(ct);

        return StatusCodes.Status200OK;
    }
}