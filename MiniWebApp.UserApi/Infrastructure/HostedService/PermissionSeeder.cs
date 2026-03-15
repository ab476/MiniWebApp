using MiniWebApp.UserApi.Services.Repositories;

namespace MiniWebApp.UserApi.Infrastructure.HostedService;

/// <summary>
/// Seeds the database with application-level claims (permissions).
/// </summary>
/// <remarks>
/// <strong>Key Responsibilities:</strong><br/>
/// <list type="bullet">
///     <item><description>Extracts statically defined permission codes from application constants.</description></item>
///     <item><description>Cross-references existing claims in the database to prevent duplicates.</description></item>
///     <item><description>Persists any missing claims with categorized descriptions.</description></item>
/// </list>
/// </remarks>
public class PermissionSeeder(IClaimRepository claimCommands, IClaimQueries claimQueries) : IDataSeeder
{
    /// <summary>
    /// Executes the asynchronous permission seeding process.
    /// </summary>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous seeding operation.</returns>
    public async Task SeedAsync(CancellationToken ct)
    {
        var permissionData = GetPermissionsToSeed();

        var existingCodesSet = await claimQueries.GetExistingClaimCodesAsync(ct);

        var newPermissions = permissionData
            .Where(p => !existingCodesSet.Contains(p.ClaimCode))
            .ToList();

        if (newPermissions.Count == 0) return;

        // Use the repository to add new claims
        await claimCommands.AddClaimsAsync(newPermissions, ct);
    }

    /// <summary>
    /// Aggregates all statically defined application permissions into a single collection.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="AppClaim"/> entities ready for seeding.</returns>
    private static IEnumerable<AppClaim> GetPermissionsToSeed()
    {
        return
        [
            .. BuildPermissions(AppPermissions.Tenants.All, nameof(AppPermissions.Tenants)),
            .. BuildPermissions(AppPermissions.Roles.All, nameof(AppPermissions.Roles)),
            .. BuildPermissions(AppPermissions.Users.All, nameof(AppPermissions.Users)),
            .. BuildPermissions(AppPermissions.Permissions.All, nameof(AppPermissions.Permissions))
        ];
    }

    /// <summary>
    /// Projects a collection of string-based permission codes into <see cref="AppClaim"/> entities.
    /// </summary>
    /// <param name="codes">The raw permission codes.</param>
    /// <param name="category">The grouping category for these permissions.</param>
    /// <returns>An enumerable collection of mapped <see cref="AppClaim"/> objects.</returns>
    private static IEnumerable<AppClaim> BuildPermissions(IEnumerable<string> codes, string category)
    {
        return codes.Select(code => new AppClaim
        {
            ClaimCode = code,
            Category = category,
            Description = $"Allows {code.Replace('.', ' ')}"
        });
    }
}