using Microsoft.AspNetCore.Identity;
using MiniWebApp.Core.Security;
using MiniWebApp.Core.Services;
using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Repositories;

public sealed class UserRepository(
    UserDbContext db,
    IPasswordHasher<User> _passwordHasher,
    ILogger<UserRepository> _logger,
    IRequestContext requestContext,
    IRoleClaimQueries roleClaimQueries,
    IScopedStateService scopedStateService) : RepositoryBase(requestContext), IUserRepository
{
    public async Task<Outcome<Guid[]>> CreateBulkAsync(IEnumerable<CreateUserRequest> requests, CancellationToken ct = default)
    {
        var userRequests = requests.ToList();
        if (userRequests.Count == 0)
        {
            return Outcome.Success(StatusCodes.Status200OK, Array.Empty<Guid>());
        }

        var newUsers = userRequests.Select(request =>
        {
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId,
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpperInvariant(),
                UserName = request.UserName,
                NormalizedUsername = request.UserName.ToUpperInvariant(),
                PasswordHash = string.Empty, // Placeholder, will be set after hashing
                CreatedAt = DateTime.UtcNow,
                Status = UserStatus.Active
            };
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

            return newUser;
        } ).ToList();

        await db.Users.AddRangeAsync(newUsers, ct);
        await db.SaveChangesAsync(ct);

        return Outcome.Success(StatusCodes.Status201Created, newUsers.Select(u => u.Id).ToArray());
    }

    public async Task<Outcome<Guid>> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var normalizedEmail = request.Email.ToUpperInvariant();
        var normalizedUsername = request.UserName.ToUpperInvariant();

        var emailExists = await db.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, ct);
        if (emailExists)
        {
            return Outcome.Failure("Email already exists.", StatusCodes.Status409Conflict);
        }

        var usernameExists = await db.Users.AnyAsync(u => u.NormalizedUsername == normalizedUsername, ct);
        if (usernameExists)
        {
            return Outcome.Failure("Username already exists.", StatusCodes.Status409Conflict);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            Email = request.Email,
            NormalizedEmail = normalizedEmail,
            UserName = request.UserName,
            NormalizedUsername = normalizedUsername,
            PasswordHash = string.Empty, // Placeholder, will be set after hashing
            CreatedAt = DateTime.UtcNow,
            Status = UserStatus.Pending
        };

        await db.Users.AddAsync(user, ct);
        await db.SaveChangesAsync(ct);

        return Outcome.Success(StatusCodes.Status201Created, user.Id);
    }

    public async Task<Outcome> UpdateStatusAsync(UpdateUserStatusRequest request, CancellationToken ct = default)
    {
        // Using ExecuteUpdateAsync for performance (consistent with your ExecuteDeleteAsync pattern)
        var rows = await db.Users // Ensure this operation is tenant-scoped
            .Where(u => u.Id == request.UserId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.Status, request.NewStatus)
                .SetProperty(u => u.UpdatedAt, DateTime.UtcNow),
                ct);

        return rows > 0
            ? Outcome.Success(StatusCodes.Status200OK)
            : Outcome.Failure("User not found.", StatusCodes.Status404NotFound);
    }

    public async Task<Outcome> SoftDeleteAsync(Guid userId, CancellationToken ct = default)
    {
        // Best practice: Don't physically delete users, change status to Deleted
        var rows = await db.Users // Ensure this operation is tenant-scoped
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.Status, UserStatus.Deleted),
                ct);

        return rows > 0 ? Outcome.Success(StatusCodes.Status200OK) : Outcome.Failure("User not found.", StatusCodes.Status404NotFound);
    }

    public async Task<Outcome<AuthenticatedUserContext>> VerifyCredentialsAsync(VerifyCredentialsRequest request, CancellationToken ct = default)
    {
        // 1. Validate input and normalize for efficient DB lookup
        var identifier = (request.Identifier)?.ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(identifier))
        {
            _logger.LogWarning("Login attempt rejected: No identifier provided.");
            return Outcome.Failure("Username or email is required.", StatusCodes.Status400BadRequest);
        }

        // 2. Single-pass fetch with Roles to avoid N+1 issues during token creation
        var user = await db.Users
            .Include(u => u.UserRoles)
            //.Where(u => u.TenantId == ContextTenantId) // CRITICAL: Filter by tenant for security
                .Include(ur => ur.UserRoles)
                    .ThenInclude(ur => ur.Role) // Include Role for token context
            .FirstOrDefaultAsync(u =>
                u.NormalizedUsername == identifier ||
                u.NormalizedEmail == identifier, ct);

        if (user is null)
        {
            _logger.LogInformation("Login failed: Identifier {Identifier} not found.", identifier);
            return Outcome.Failure("Invalid credentials.", StatusCodes.Status401Unauthorized);
        }

        // 3. Security Guardrails (Fail fast before expensive hashing)
        if (user.Status != UserStatus.Active)
        {
            _logger.LogWarning("Login blocked: User {UserId} has status {Status}.", user.Id, user.Status);
            return ($"Account is {user.Status.ToString().ToLower()}.", StatusCodes.Status403Forbidden);
        }

        if (user.LockoutEnd > DateTime.UtcNow)
        {
            _logger.LogWarning("Login blocked: User {UserId} is locked out until {LockoutEnd}.", user.Id, user.LockoutEnd);
            return Outcome.Failure("Account is temporarily locked.", StatusCodes.Status423Locked);
        }

        // 4. Password Verification
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                _logger.LogWarning("User {UserId} locked out due to repeated failures.", user.Id);
            }

            await db.SaveChangesAsync(ct);
            return Outcome.Failure("Invalid credentials.", StatusCodes.Status401Unauthorized);
        }

        // 5. Success Path & State Maintenance
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            _logger.LogInformation("Password rehashed for User {UserId}.", user.Id);
        }

        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        _logger.LogInformation("Successful login: User {UserId} (Tenant: {TenantId}).", user.Id, user.TenantId);
        scopedStateService.Set<JwtUser>(new JwtUser(
            UserId: user.Id,
            Email: user.Email,
            UserName: user.UserName,
            TenantId: user.TenantId,
            Roles: [],
            Permissions: []));
        var claims = await roleClaimQueries.GetClaimsByRolesAsync([.. user.UserRoles.Select(ur => ur.RoleCode)], ct);
        // 6. Return Context for Token Generation
        var userTokenContext = new AuthenticatedUserContext(
            user.Id,
            user.TenantId,
            user.Email,
            user.UserName,
            [.. user.UserRoles.Select(ur => ur.Role.RoleCode)],
            [.. claims.Value!.Select(c => c.ClaimCode)]
        );
        return Outcome.Success(StatusCodes.Status200OK, userTokenContext);
    }

}
public record AuthenticatedUserContext(
    Guid UserId,
    Guid TenantId,
    string Email,
    string Username,
    List<string> Roles,
    string[] Permissions
);

public record VerifyCredentialsRequest(string Identifier, string Password);