using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MiniWebApp.Core.Security;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Services.Repositories;
using MiniWebApp.UserApi.Options;

namespace MiniWebApp.UserApi.Infrastructure.HostedService;

/// <summary>
/// Seeds the database with initial user accounts and assigns their default roles.
/// </summary>
public class UserSeeder(
    IUserRepository userRepository,
    IUserQueries userQueries,
    IUserRoleRepository userRoleRepository,
    IUserContext userContext,
    IOptions<SeedDataOptions> options,
    ILogger<UserSeeder> logger) : IDataSeeder
{
    private readonly SeedDataOptions _seedData = options.Value;

    public async Task SeedAsync(CancellationToken ct)
    {
        if (_seedData.Users.Count == 0)
        {
            logger.LogInformation("No users found in seed data. Skipping user seeding.");
            return;
        }

        logger.LogInformation("Starting user seeding process.");

        try
        {
            var defaultTenantId = userContext.TenantId;

            var validRoleCodes = _seedData.Roles
                .Select(r => r.RoleCode)
                .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

            var existingEmails = await userQueries.GetExistingUserEmailsAsync(ct);

            var usersToCreate = new List<CreateUserRequest>();
            var userRolesToAssign = new List<(string Email, string[] RoleCodes)>();

            foreach (var userSeed in _seedData.Users)
            {
                if (existingEmails.Value!.Contains(userSeed.Email))
                {
                    logger.LogWarning("Skipping user with email {Email} as it already exists.", userSeed.Email);
                    continue;
                }

                usersToCreate.Add(new CreateUserRequest(userSeed.Email, userSeed.Email, userSeed.Password, defaultTenantId));

                var rolesForUser = userSeed.Roles
                    .Where(validRoleCodes.Contains)
                    .ToArray();

                if (rolesForUser.Length > 0)
                {
                    userRolesToAssign.Add((userSeed.Email, rolesForUser));
                }
            }

            if (usersToCreate.Count == 0)
            {
                logger.LogInformation("No new users to seed.");
                return;
            }

            // 1. Create Users
            var createUsersResult = await userRepository.CreateBulkAsync(usersToCreate, ct);
            if (!createUsersResult.IsSuccess)
            {
                logger.LogError("Failed to create users during seeding: {Error}", createUsersResult.Error);
                return;
            }

            // 2. Fetch the newly created users to get their IDs
            var newlyCreatedUserEmails = usersToCreate.Select(u => u.Email).ToHashSet(StringComparer.InvariantCultureIgnoreCase);
            var newlyCreatedUsersOutcome = await userQueries.GetByEmailsAsync(newlyCreatedUserEmails, ct);

            if (!newlyCreatedUsersOutcome.IsSuccess)
            {
                logger.LogError("Failed to retrieve newly created users during seeding: {Error}", newlyCreatedUsersOutcome.Error);
                return;
            }

            // 3. Map Roles to the new User IDs
            var emailToIdMap = newlyCreatedUsersOutcome.Value.ToDictionary(u => u.Email, u => u.Id, StringComparer.InvariantCultureIgnoreCase);
            var userRoles = new List<UserRole>();

            foreach (var (email, roles) in userRolesToAssign)
            {
                if (emailToIdMap.TryGetValue(email, out var userId))
                {
                    userRoles.AddRange(roles.Select(role => new UserRole { UserId = userId, RoleCode = role, TenantId = defaultTenantId }));
                }
            }

            // 4. Assign Roles
            if (userRoles.Count > 0)
            {
                var outcome = await userRoleRepository.CreateBulkAsync(userRoles, ct);
                if (!outcome.IsSuccess)
                {
                    logger.LogError("Failed to assign roles during seeding: {Error}", outcome.Error);
                    return;
                }
            }

            logger.LogInformation("Successfully seeded {Count} users.", usersToCreate.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during user seeding.");
        }
    }
}