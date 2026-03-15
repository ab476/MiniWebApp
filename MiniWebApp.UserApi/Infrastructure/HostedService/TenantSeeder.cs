using Microsoft.Extensions.Options;
using MiniWebApp.Core.Security;
using MiniWebApp.UserApi.Options;
using MiniWebApp.UserApi.Services.Repositories;

namespace MiniWebApp.UserApi.Infrastructure.HostedService;

/// <summary>
/// Seeds the database with initial multi-tenancy records.
/// </summary>
/// <remarks> 
/// <strong>Key Responsibilities:</strong> 
/// <list type="bullet"> 
///     <item><description>Reads tenant configurations from the application's startup options.</description></item> 
///     <item><description>Cross-references existing tenant names in the database to prevent duplication.</description></item> 
///     <item><description>Generates base tenant entities with an active status and new unique identifiers.</description></item> 
/// </list> 
/// </remarks> 
/// <param name="tenantRepository">The repository for managing tenant data.</param>
/// <param name="tenantQueries">The service for querying tenant data.</param>
/// <param name="options">The application settings containing the seed data configuration.</param>
public class TenantSeeder(
    ITenantRepository tenantRepository,
    ITenantQueries tenantQueries,
    IOptions<SeedDataOptions> options,
    ILogger<TenantSeeder> logger) : IDataSeeder
{
    private readonly SeedDataOptions _seedData = options.Value;
    private readonly ILogger<TenantSeeder> _logger = logger;

    /// <summary>
    /// Executes the asynchronous tenant seeding process.
    /// </summary>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous seeding operation.</returns>
    public async Task SeedAsync(CancellationToken ct)
    {
        if (_seedData.Tenants.Count == 0) return;

        var existingNames = await tenantQueries.GetExistingTenantNamesAsync(ct);

        var newTenants = _seedData.Tenants
            .Where(t => !existingNames.Contains(t.Name))
            .Select(MapToEntity)
            .ToList();

        if (newTenants.Count == 0) return;

        var createResult = await tenantRepository.CreateBulkAsync(newTenants, ct);

        if (!createResult.IsSuccess)
        {
            _logger.LogError("Failed to seed tenants. Status: {StatusCode}, Error: {Error}",
                createResult.StatusCode, createResult.Error);
            return;
        }
    }

    /// <summary>
    /// Maps a configuration-based tenant seed model into a database entity.
    /// </summary>
    /// <param name="seed">The source configuration model to map.</param>
    /// <returns>A new <see cref="Tenant"/> entity ready for persistence.</returns>
    private static Tenant MapToEntity(TenantSeed seed) => new()
    {
        Id = Guid.NewGuid(),
        Name = seed.Name,
        Domain = seed.Domain,
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };
}