using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Domain.Models;
using MiniWebApp.UserApi.Options;

namespace MiniWebApp.UserApi.HostedService;

public class UserSeeder(
    UserDbContext dbContext,
    IPasswordHasher<TUser> passwordHasher,
    IOptions<SeedDataOptions> options) : SeederBase(dbContext), IUserSeeder
{
    private readonly SeedDataOptions _seedData = options.Value;

    public async Task SeedAsync(CancellationToken ct)
    {
        if (_seedData.Users.Count == 0) return;

        // 1. Get the Default Tenant (Assume first one or name "System/Default")
        var defaultTenantId = await GetDefaultTenantIdAsync(ct);

        // 2. Map Roles to IDs
        var roleMap = await _dbContext.Set<TRole>()
            .ToDictionaryAsync(r => r.Name, r => r.Id, ct);

        // 3. Get existing users
        var existingEmails = await _dbContext.Set<TUser>()
            .Select(u => u.Email)
            .ToHashSetAsync(ct);

        foreach (var userSeed in _seedData.Users)
        {
            if (existingEmails.Contains(userSeed.Email)) continue;

            var newUser = new TUser
            {
                Id = Guid.NewGuid(),
                Email = userSeed.Email,
                UserName = userSeed.Email,
                TenantId = defaultTenantId,
                EmailConfirmed = true,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            // Hash the password securely
            newUser.PasswordHash = passwordHasher.HashPassword(newUser, userSeed.Password);

            // 4. Assign Roles
            var userRoles = userSeed.Roles
                .Where(roleMap.ContainsKey)
                .Select(roleName => new TUserRole
                {
                    UserId = newUser.Id,
                    RoleId = roleMap[roleName]
                })
                .ToList();

            await _dbContext.Set<TUser>().AddAsync(newUser, ct);
            if (userRoles.Count != 0)
            {
                await _dbContext.Set<TUserRole>().AddRangeAsync(userRoles, ct);
            }
        }

        await _dbContext.SaveChangesAsync(ct);
    }
}
