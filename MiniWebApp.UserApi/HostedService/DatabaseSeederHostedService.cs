using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiniWebApp.UserApi.Application;
using MiniWebApp.UserApi.Application.Tenants;
using MiniWebApp.UserApi.Contracts.Roles;
using MiniWebApp.UserApi.Contracts.Tenants;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Domain.Models;
using MiniWebApp.UserApi.Models.Roles;
using MiniWebApp.UserApi.Models.Tenants;
using MiniWebApp.UserApi.Options;

namespace MiniWebApp.UserApi.HostedService;

public sealed class DatabaseSeederHostedService(
    IServiceProvider provider,
    IOptions<SeedDataOptions> options,
    ILogger<DatabaseSeederHostedService> logger) : IHostedService
{
    private readonly IServiceProvider _provider = provider;
    private readonly SeedDataOptions _options = options.Value;
    private readonly ILogger<DatabaseSeederHostedService> _logger = logger;

    public async Task StartAsync(CancellationToken ct)
    {
        using var scope = _provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<TUser>>();

        var tenatService = scope.ServiceProvider.GetRequiredService<ITenantService>();
        var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();

        await SeedTenantsAsync(db, tenatService, ct);
        await SeedRolesAsync(db, roleService, ct);
        await SeedUsersAsync(db, passwordHasher, ct);

        await db.SaveChangesAsync(ct);

        _logger.LogInformation("Database seeding completed.");
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;

    // -------------------------------
    // Seeding Methods
    // -------------------------------

    private readonly List<TenantResponse> _cachedTenants = [];
    private async Task SeedTenantsAsync(UserDbContext db, ITenantService tenantService, CancellationToken ct)
    {
        if (db.TTenants.Any())
            return;
        foreach (var tenant in _options.Tenants)
        {
            var tenantRes = await tenantService.CreateAsync(new CreateTenantRequest { Name = tenant.Name, Domain = tenant.Domain }, ct);
            _cachedTenants.Add(tenantRes.Value!);
        }
    }

    private async Task SeedRolesAsync(UserDbContext db, IRoleService roleService, CancellationToken ct)
    {
        if (await db.TRoles.AnyAsync(ct))
            return;

        foreach (var tenant in _cachedTenants)
        {
            foreach (var role in _options.Roles)
            {
                await roleService.CreateAsync(
                    new CreateRoleRequest 
                    { 
                        TenantId = tenant.Id, 
                        Name = role.Name, 
                        Description = null 
                    }, 
                    ct
                );
                
            }
        }
    }

    private async Task SeedUsersAsync(
        UserDbContext db,
        IPasswordHasher<TUser> hasher,
        CancellationToken ct)
    {
        foreach (var userSeed in _options.Users)
        {
            if (await db.TUsers.AnyAsync(u => u.Email == userSeed.Email, ct))
                continue;

            var user = new TUser
            {
                Id = Guid.NewGuid(),
                Email = userSeed.Email,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = hasher.HashPassword(user, userSeed.Password);

            foreach (var roleName in userSeed.Roles)
            {
                var role = await db.TRoles
                    .FirstOrDefaultAsync(r => r.Name == roleName, ct);

                if (role is null)
                    continue;

                user.UserRoles.Add(new TUserRole
                {
                    RoleId = role.Id
                });
            }

            db.TUsers.Add(user);
        }
    }
}

