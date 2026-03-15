using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Repositories;

public interface IClaimRepository
{
    
    /// <summary>
    /// Adds a collection of new claims to the database and saves changes.
    /// </summary>
    Task AddClaimsAsync(IEnumerable<AppClaim> claims, CancellationToken ct);
}


public sealed class ClaimRepository(UserDbContext db) : IClaimRepository
{
    /// <inheritdoc />
    public async Task AddClaimsAsync(IEnumerable<AppClaim> claims, CancellationToken ct)
    {
        await db.Claims.AddRangeAsync(claims, ct);
        await db.SaveChangesAsync(ct);
    }
}