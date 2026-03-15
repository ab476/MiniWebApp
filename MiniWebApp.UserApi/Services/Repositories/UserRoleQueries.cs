using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Repositories;


public sealed record UserRoleResponse
{
    public Guid UserId { get; init; }
    public required string RoleCode { get; init; }
    public string RoleName { get; init; } = default!;
}

public sealed record UserRoleRequest
{
    public Guid UserId { get; init; }
    public required string RoleCode { get; init; }
}

public sealed record BulkUserRoleRequest
{
    public Guid UserId { get; init; }
    public string[] RoleCodes { get; init; } = [];
}


public sealed class UserRoleQueries(UserDbContext db)
{
    public async Task<Outcome<IReadOnlyList<UserRoleResponse>>> GetRolesByUserAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        var roles = await db.UserRoles
            .TagWith("UserRoleQueries.GetRolesByUserAsync: Fetching roles for user")
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role) 
            .Select(ur => new UserRoleResponse
            {
                UserId = ur.UserId,
                RoleCode = ur.RoleCode,
                RoleName = ur.Role.DisplayName! // The user-friendly name of the role.
            })
            .ToListAsync(ct);

        return (StatusCodes.Status200OK, roles);
    }

    public async Task<Outcome<bool>> IsInRoleAsync(
        UserRoleRequest request,
        CancellationToken ct = default)
    {
        var exists = await db.UserRoles
            .TagWith("UserRoleQueries.IsInRoleAsync: Checking membership")
            .AsNoTracking()
            .AnyAsync(ur => ur.UserId == request.UserId && ur.RoleCode == request.RoleCode, ct);

        return (StatusCodes.Status200OK, exists);
    }
}
