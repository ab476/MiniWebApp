using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MiniWebApp.Core.Security;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Options;

namespace MiniWebApp.UserApi.Infrastructure.HostedService;

/// <summary>
/// Seeds the database with initial user accounts and assigns their default roles.
/// </summary>
/// <remarks>
/// <strong>Key Responsibilities:</strong>
/// <list type="bullet">
///     <item><description>Resolves the default system tenant to associate with system-level users.</description></item>
///     <item><description>Cross-references existing emails to prevent duplicate account creation.</description></item>
///     <item><description>Securely hashes passwords using the configured <see cref="IPasswordHasher{TUser}"/>.</description></item>
///     <item><description>Validates and maps assigned roles to ensure foreign key integrity.</description></item>
/// </list>
/// </remarks>
/// <param name="dbContext">The database context used for data access.</param>
/// <param name="passwordHasher">The service used to securely hash user passwords before storage.</param>
/// <param name="userContext">The context providing the current user information.</param>
/// <param name="options">The application settings containing the seed data configuration.</param>
public class UserSeeder(
    UserDbContext _dbContext,
    IPasswordHasher<User> passwordHasher,
    IUserContext userContext,
    IOptions<SeedDataOptions> options) : IDataSeeder
{
    private readonly SeedDataOptions _seedData = options.Value;

    /// <summary>
    /// Executes the asynchronous user seeding process.
    /// </summary>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous seeding operation.</returns>
    /// <remarks>
    /// <strong>Execution Flow:</strong>
    /// <list type="number">
    ///     <item><description>Checks if user seed data exists in the configuration.</description></item>
    ///     <item><description>Resolves the default tenant context.</description></item>
    ///     <item><description>Retrieves existing roles and user emails into case-insensitive hash sets for fast validation.</description></item>
    ///     <item><description>Iterates over the seed configuration to build <see cref="User"/> and <see cref="UserRole"/> collections.</description></item>
    ///     <item><description>Executes a batch insert to persist the new users and their role mappings to the database.</description></item>
    /// </list>
    /// </remarks>
    public async Task SeedAsync(CancellationToken ct)
    {
        if (_seedData.Users.Count == 0) return;

        var defaultTenantId = userContext.TenantId;

        var validRoleCodes = await _dbContext.Set<Role>()
            .Select(r => r.RoleCode)
            .ToHashSetAsync(StringComparer.InvariantCultureIgnoreCase, ct);

        var existingEmails = await _dbContext.Set<User>()
            .Select(u => u.Email)
            .ToHashSetAsync(StringComparer.InvariantCultureIgnoreCase, ct);

        var newUsers = new List<User>();
        var newUserRoles = new List<UserRole>();

        foreach (var userSeed in _seedData.Users)
        {
            if (existingEmails.Contains(userSeed.Email)) continue;

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = userSeed.Email,
                NormalizedEmail = userSeed.Email.ToUpperInvariant(),
                UserName = userSeed.Email,
                NormalizedUsername = userSeed.Email.ToUpperInvariant(),
                TenantId = defaultTenantId,
                EmailConfirmed = true,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = string.Empty // Placeholder, will be set after hashing
            };
            newUser.PasswordHash = passwordHasher.HashPassword(newUser, userSeed.Password);
            newUsers.Add(newUser);

            var userRoles = userSeed.Roles
                .Where(validRoleCodes.Contains)
                .Select(roleCode => new UserRole
                {
                    UserId = newUser.Id,
                    RoleCode = roleCode
                });

            newUserRoles.AddRange(userRoles);
        }

        if (newUsers.Count == 0) return;

        await _dbContext.Set<User>().AddRangeAsync(newUsers, ct);

        if (newUserRoles.Count > 0)
        {
            await _dbContext.Set<UserRole>().AddRangeAsync(newUserRoles, ct);
        }

        await _dbContext.SaveChangesAsync(ct);
    }
}