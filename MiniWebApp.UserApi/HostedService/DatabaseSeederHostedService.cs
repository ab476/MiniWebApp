using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Options;

namespace MiniWebApp.UserApi.HostedService;

public sealed class DatabaseSeederHostedService(
    IServiceProvider provider,
    IOptions<SeedDataOptions> options,
    ILogger<DatabaseSeederHostedService> logger,
    IWebHostEnvironment environment) : IHostedService
{
    private readonly IServiceProvider _provider = provider;
    private readonly SeedDataOptions _options = options.Value;
    private readonly ILogger<DatabaseSeederHostedService> _logger = logger;

    public async Task StartAsync(CancellationToken ct)
    {
        _logger.LogInformation("Starting database seeding process...");

        using var scope = _provider.CreateScope();

        if (environment.IsDevelopment())
        {
            _logger.LogInformation("Development environment detected. Resetting database...");

            var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            // 1. Wipe the existing database
            await context.Database.EnsureDeletedAsync(ct);

            // 2. Re-apply all migrations to create a fresh schema
            await context.Database.EnsureCreatedAsync(ct);

            _logger.LogInformation("Database schema recreated.");
        }
        var permissionSeeder = scope.ServiceProvider.GetRequiredService<IPermissionSeeder>();
        await permissionSeeder.SeedAsync(ct);

        var tenantSeeder = scope.ServiceProvider.GetRequiredService<ITenantSeeder>();
        await tenantSeeder.SeedAsync(ct);

        var roleSeeder = scope.ServiceProvider.GetRequiredService<IRoleSeeder>();
        await roleSeeder.SeedAsync(ct);

        var userSeeder = scope.ServiceProvider.GetRequiredService<IUserSeeder>();
        await userSeeder.SeedAsync(ct);

        _logger.LogInformation("Database seeding completed.");
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
   
}

