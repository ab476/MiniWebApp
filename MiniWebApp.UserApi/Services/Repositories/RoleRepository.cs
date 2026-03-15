using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Repositories;

/// <summary>
/// Provides data access and management for roles with multi-tenant security validation.
/// </summary>
/// <param name="db">The database context for user-related data.</param>
/// <param name="requestContext">The ambient request context.</param>
public sealed class RoleRepository(UserDbContext db, IRequestContext requestContext)
    : RepositoryBase(requestContext), IRoleRepository
{
    /// <summary>
    /// Validates if the current user has permission to perform actions on the target tenant.
    /// </summary>
    private Outcome ValidateTenantAccess(Guid? requestTenantId)
    {
        if (!requestTenantId.HasValue || requestTenantId == ContextTenantId)
        {
            return StatusCodes.Status200OK;
        }

        if (!IsSuperAdmin)
        {
            return ("Access Denied: You do not have permission to manage roles for another tenant.",
                    StatusCodes.Status403Forbidden);
        }

        return StatusCodes.Status200OK;
    }

    /// <inheritdoc />
    public async Task<Outcome<RoleResponse>> CreateAsync(CreateRoleRequest request, CancellationToken ct = default)
    {
        var validation = ValidateTenantAccess(request.TenantId);
        if (!validation.IsSuccess) return validation.ToFailure<RoleResponse>();

        var role = new Role
        {
            TenantId = request.TenantId ?? ContextTenantId,
            RoleCode = request.RoleCode,
            DisplayName = request.DisplayName,
            CreatedAt = UtcNow
        };

        await db.Roles.AddAsync(role, ct);
        await db.SaveChangesAsync(ct);

        return (StatusCodes.Status201Created, role.ToResponse());
    }

    /// <inheritdoc />
    public async Task<Outcome<RoleResponse[]>> CreateBulkAsync(IEnumerable<CreateRoleRequest> requests, CancellationToken ct = default)
    {
        var requestList = requests.ToList();
        if (requestList.Count == 0) return (StatusCodes.Status200OK, []);

        var targetTenantId = requestList[0].TenantId ?? ContextTenantId;

        if (targetTenantId != ContextTenantId && !IsSuperAdmin)
        {
            return ("Access Denied: You do not have permission to manage roles for another tenant.",
                    StatusCodes.Status403Forbidden);
        }

        if (requestList.Any(r => (r.TenantId ?? ContextTenantId) != targetTenantId))
        {
            return ("Bulk operations across multiple different tenants are not permitted in a single request.",
                    StatusCodes.Status400BadRequest);
        }

        var roles = requestList.Select(request => new Role
        {
            TenantId = targetTenantId,
            RoleCode = request.RoleCode,
            DisplayName = request.DisplayName,
            CreatedAt = UtcNow
        }).ToList();

        await db.Roles.AddRangeAsync(roles, ct);
        await db.SaveChangesAsync(ct);

        return (StatusCodes.Status201Created, [.. roles.Select(r => r.ToResponse())]);
    }

    /// <inheritdoc />
    public async Task<Outcome> UpdateAsync(UpdateRoleRequest request, CancellationToken ct = default)
    {
        var validation = ValidateTenantAccess(request.TenantId);
        if (!validation.IsSuccess) return validation;

        var effectiveTenantId = request.TenantId ?? ContextTenantId;
        var rows = await db.Roles
            .Where(r => r.RoleCode == request.RoleCode && r.TenantId == effectiveTenantId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(r => r.DisplayName, request.DisplayName), ct);

        return rows == 1
            ? StatusCodes.Status200OK
            : ("Role not found.", StatusCodes.Status404NotFound);
    }

    /// <inheritdoc />
    public async Task<Outcome> DeleteAsync(DeleteRoleRequest request, CancellationToken ct = default)
    {
        var validation = ValidateTenantAccess(request.TenantId);
        if (!validation.IsSuccess) return validation;

        var effectiveTenantId = request.TenantId ?? ContextTenantId;
        var rows = await db.Roles
            .Where(r => r.RoleCode == request.RoleCode && r.TenantId == effectiveTenantId)
            .ExecuteDeleteAsync(ct);

        return rows == 1
            ? StatusCodes.Status200OK
            : ("Role not found.", StatusCodes.Status404NotFound);
    }
}