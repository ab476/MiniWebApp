using Microsoft.Extensions.Options;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Options;

namespace MiniWebApp.UserApi.Infrastructure.HostedService;

public class TenantSeeder(
    UserDbContext dbContext,
    IOptions<SeedDataOptions> options) : SeederBase(dbContext), ITenantSeeder
{
    private readonly SeedDataOptions _seedData = options.Value;

    public async Task SeedAsync(CancellationToken ct)
    {
        if (_seedData.Tenants.Count == 0) return;

        var existingNames = await _dbContext.Tenants
            .Select(t => t.Name)
            .ToHashSetAsync(ct);

        var newTenants = _seedData.Tenants
            .Where(t => !existingNames.Contains(t.Name))
            .Select(MapToEntity)
            .ToList();

        if (newTenants.Count != 0)
        {
            await _dbContext.Tenants.AddRangeAsync(newTenants, ct);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    private static Tenant MapToEntity(TenantSeed seed) => new()
    {
        Id = Guid.NewGuid(),
        Name = seed.Name,
        Domain = seed.Domain,
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };
}