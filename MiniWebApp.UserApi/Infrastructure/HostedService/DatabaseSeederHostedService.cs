using Microsoft.Extensions.Options;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Options;

namespace MiniWebApp.UserApi.Infrastructure.HostedService;

/// <summary>
/// Orchestrates the database seeding pipeline as a background service during application startup.
/// </summary>
/// <remarks>
/// <strong>Key Responsibilities:</strong>
/// <list type="bullet">
///     <item><description>Evaluates configuration to determine if seeding should proceed.</description></item>
///     <item><description>Manages dependency injection scopes for transient seeding operations.</description></item>
///     <item><description>Wipes and recreates the database schema when running in a Development environment.</description></item>
///     <item><description>Discovers and executes all registered <see cref="IDataSeeder"/> implementations sequentially.</description></item>
/// </list>
/// </remarks>
/// <param name="provider">The service provider used to create scopes and resolve scoped seeders.</param>
/// <param name="options">The application settings containing the global seed data configuration.</param>
/// <param name="logger">The logger used to output seeding progress and structural changes.</param>
/// <param name="environment">The hosting environment used to detect if destructive operations (like DB reset) are permitted.</param>
public sealed class DatabaseSeederHostedService(
    IServiceProvider provider,
    IOptions<SeedDataOptions> options,
    ILogger<DatabaseSeederHostedService> logger,
    IWebHostEnvironment environment) : IHostedService
{
    private readonly SeedDataOptions _options = options.Value;

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous startup operation.</returns>
    public async Task StartAsync(CancellationToken ct)
    {
        if (!_options.Enabled) return;

        logger.LogInformation("Starting database seeding process...");

        using var scope = provider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            if (environment.IsDevelopment())
            {
                await PrepareDatabaseAsync(services, ct);
            }

            var seeders = services.GetServices<IDataSeeder>();
            foreach (var seeder in seeders)
            {
                logger.LogInformation("Executing seeder: {SeederName}", seeder.GetType().Name);
                await seeder.SeedAsync(ct);
            }

            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database seeding.");
            throw;
        }
    }

    /// <summary>
    /// Prepares the database for seeding by ensuring a clean schema exists. 
    /// This is a destructive operation intended strictly for development environments.
    /// </summary>
    private async Task PrepareDatabaseAsync(IServiceProvider services, CancellationToken ct)
    {
        var context = services.GetRequiredService<UserDbContext>();
        logger.LogInformation("Resetting database for Development...");
        await context.Database.EnsureDeletedAsync(ct);
        await context.Database.EnsureCreatedAsync(ct);
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}