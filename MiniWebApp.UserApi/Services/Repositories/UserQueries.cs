using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Repositories;


public sealed record UserResponse(
    Guid Id,
    string Email,
    string UserName,
    UserStatus Status,
    DateTime CreatedAt
);

public sealed record CreateUserRequest(
    string Email,
    string UserName,
    string Password,
    Guid TenantId
);

public sealed record UpdateUserStatusRequest(Guid UserId, UserStatus NewStatus);
public sealed class UserQueries(UserDbContext db) : IUserQueries
{
    public async Task<Outcome<UserResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await db.Users
            .TagWith("UserQueries.GetByIdAsync: Fetching user by ID")
            .AsNoTracking()
            .Where(u => u.Id == id)
            .ProjectToResponse()
            .FirstOrDefaultAsync(ct);

        return user is not null
            ? (StatusCodes.Status200OK, user)
            : ("User not found.", StatusCodes.Status404NotFound);
    }

    public async Task<Outcome<IReadOnlyList<UserResponse>>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default)
    {
        var users = await db.Users
            .TagWith("UserQueries.GetByTenantAsync: Listing users for tenant")
            .AsNoTracking()
            .Where(u => u.TenantId == tenantId && u.Status != UserStatus.Deleted)
            .ProjectToResponse()
            .ToListAsync(ct);

        return (StatusCodes.Status200OK, users);
    }
    public async Task<HashSet<string>> GetExistingUserEmailsAsync(CancellationToken ct = default)
    {
        return await db.Users
            .AsNoTracking()
            .TagWith("UserQueries.GetExistingUserEmailsAsync: Fetching existing user emails")
            .Select(u => u.Email)
            .ToHashSetAsync(StringComparer.InvariantCultureIgnoreCase, ct);
    }
}
