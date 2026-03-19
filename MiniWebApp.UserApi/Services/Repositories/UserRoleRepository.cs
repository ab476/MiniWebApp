using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Repositories;

public sealed class UserRoleRepository(
    UserDbContext db, 
    IRequestContext requestContext
    ) : RepositoryBase(requestContext), IUserRoleRepository
    {
    public async Task<Outcome> CreateBulkAsync(IEnumerable<UserRole> userRoles, CancellationToken ct = default)
    {
        var roleList = userRoles.ToList();
        if (roleList.Count == 0)
        {
            return Outcome.Success(StatusCodes.Status200OK);
        }

        db.UserRoles.AddRange(roleList);
        await db.SaveChangesAsync(ct);

        return Outcome.Success(StatusCodes.Status201Created);
    }
    public async Task<Outcome> AssignAsync(UserRoleRequest request, CancellationToken ct = default)
    {
        var exists = await db.UserRoles
            .AnyAsync(ur => ur.TenantId == ContextTenantId && ur.UserId == request.UserId && ur.RoleCode == request.RoleCode, ct);

        if (exists) return Outcome.Failure("User is already assigned to this role.", StatusCodes.Status409Conflict);

        var assignment = new UserRole
        {
            UserId = request.UserId,
            RoleCode = request.RoleCode,
            TenantId = ContextTenantId
        };

        db.UserRoles.Add(assignment);
        await db.SaveChangesAsync(ct);

        return Outcome.Success(StatusCodes.Status201Created);
    }

    public async Task<Outcome> AssignBulkAsync(BulkUserRoleRequest request, CancellationToken ct = default)
    {
        // 1. Get existing role assignments for the user
        var existingRoleIds = await db.UserRoles
            .AsNoTracking()
            .Where(ur => ur.TenantId == ContextTenantId && ur.UserId == request.UserId)
            .Select(ur => ur.RoleCode)
            .ToListAsync(ct);

        // 2. Filter for new roles only
        var newAssignments = request.RoleCodes
            .Distinct()
            .Except(existingRoleIds, StringComparer.OrdinalIgnoreCase) // Use StringComparer for case-insensitive comparison
            .Select(rId => new UserRole
            {
                UserId = request.UserId,
                RoleCode = rId
            })
            .ToList();

        if (newAssignments.Count == 0)
        {
            return Outcome.Success(StatusCodes.Status200OK);
        }

        db.UserRoles.AddRange(newAssignments);
        await db.SaveChangesAsync(ct);

        return Outcome.Success(StatusCodes.Status201Created);
    }

    public async Task<Outcome> RevokeAsync(UserRoleRequest request, CancellationToken ct = default)
    {
        var rows = await db.UserRoles
            .TagWith("UserRoleRepository.RevokeAsync")
            .Where(ur => ur.TenantId == ContextTenantId && ur.UserId == request.UserId && ur.RoleCode == request.RoleCode)
            .ExecuteDeleteAsync(ct);

        return rows > 0 ? Outcome.Success(StatusCodes.Status200OK) : Outcome.Failure("Role not found.", StatusCodes.Status404NotFound);
    }

    public async Task<Outcome> RevokeAllForUserAsync(Guid userId, CancellationToken ct = default)
    {
        await db.UserRoles
            .TagWith("UserRoleRepository.RevokeAllForUserAsync")
            .Where(ur => ur.TenantId == ContextTenantId && ur.UserId == userId)
            .ExecuteDeleteAsync(ct);

        return Outcome.Success(StatusCodes.Status200OK);
    }
}