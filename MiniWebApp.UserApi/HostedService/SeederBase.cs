using Microsoft.EntityFrameworkCore;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Domain.Models;

namespace MiniWebApp.UserApi.HostedService;

public class SeederBase(UserDbContext dbContext)
{
    protected readonly UserDbContext _dbContext = dbContext;
    /// <summary>
    /// Get the Default Tenant (Assume first one or name "System/Default")
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected async Task<Guid> GetDefaultTenantIdAsync(CancellationToken ct)
    {
        // Try to find a "System" or "Default" tenant first
        var tenantId = await _dbContext.Set<TTenant>()
            .Where(t => t.Name == "System" || t.Name == "Default")
            .Select(t => (Guid?)t.Id)
            .FirstOrDefaultAsync(ct);

        // Fallback: If no system tenant exists, grab the very first available tenant
        tenantId ??= await _dbContext.Set<TTenant>()
            .Select(t => (Guid?)t.Id)
            .FirstOrDefaultAsync(ct);

        // Ensure we actually found a result before returning
        return tenantId
            ?? throw new InvalidOperationException("Seeding failed: No Tenant found. Ensure TenantSeeder runs before RoleSeeder.");
    }
}