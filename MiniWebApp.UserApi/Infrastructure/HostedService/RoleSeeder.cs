using Microsoft.Extensions.Options;
using MiniWebApp.Core.Security;
using MiniWebApp.UserApi.Options;
using MiniWebApp.UserApi.Services.Repositories;

namespace MiniWebApp.UserApi.Infrastructure.HostedService;

/// <summary>
/// Seeds the database with initial role definitions and their associated application claims.
/// </summary>
/// <param name="roleRepository">The repository for executing role creation commands.</param>
/// <param name="roleQueries">The service for querying existing role data.</param>
/// <param name="claimQueries">The service for querying existing application claims.</param>
/// <param name="roleClaimRepository">The repository for managing bulk role claim assignments.</param>
/// <param name="userContext">The context providing current user and tenant information.</param>
/// <param name="options">The configuration options containing the application's seed data.</param>
/// <param name="identitySetup">The service responsible for elevating the current scope to a system-level context.</param>
/// <param name="logger">The logger used to record execution details and potential seeding errors.</param>
public class RoleSeeder(
    IRoleRepository roleRepository,
    IRoleQueries roleQueries,
    IClaimQueries claimQueries,
    IRoleClaimRepository roleClaimRepository,
    IUserContext userContext,
    IOptions<SeedDataOptions> options,
    ISystemIdentitySetupService identitySetup,
    ILogger<RoleSeeder> logger) : IDataSeeder
{
    private readonly SeedDataOptions _seedData = options.Value;

    /// <summary>
    /// Executes the asynchronous role seeding process.
    /// </summary>
    /// <remarks>
    /// Elevates the execution context, evaluates existing roles to prevent duplicates, 
    /// creates any missing roles in bulk, and concurrently synchronizes claims for all seeded roles.
    /// </remarks>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous seeding operation.</returns>
    public async Task SeedAsync(CancellationToken ct)
    {
        if (_seedData.Roles.Count == 0) return;

        await identitySetup.SetupSystemContextAsync(ct);
        var tenantId = userContext.TenantId;

        var existingRoleCodes = await roleQueries.GetExistingRoleCodesAsync(tenantId, ct);
        var validClaimCodes = await claimQueries.GetExistingClaimCodesAsync(ct);

        var newRoles = new List<CreateRoleRequest>();
        var roleClaimsToSync = new List<BulkRoleClaimRequest>();

        foreach (var roleSeed in _seedData.Roles)
        {
            if (!existingRoleCodes.Contains(roleSeed.RoleCode))
            {
                newRoles.Add(new CreateRoleRequest(roleSeed.RoleCode, roleSeed.Name, tenantId));
            }

            var permissionCodesToAssign = roleSeed.IncludeAll
                ? validClaimCodes
                : roleSeed.Permissions.Where(validClaimCodes.Contains);

            roleClaimsToSync.Add(new BulkRoleClaimRequest(roleSeed.RoleCode, tenantId, [.. permissionCodesToAssign]));
        }

        if (newRoles.Count > 0)
        {
            var createResult = await roleRepository.CreateBulkAsync(newRoles, ct);

            if (!createResult.IsSuccess)
            {
                logger.LogError("Failed to seed roles. Status: {StatusCode}, Error: {Error}",
                    createResult.StatusCode, createResult.Error);
                return;
            }
        }

        if (roleClaimsToSync.Count > 0)
        {
            var claimTasks = roleClaimsToSync.Select(roleClaim =>
                roleClaimRepository.AssignBulkAsync(roleClaim, ct));

            await Task.WhenAll(claimTasks);
        }
    }
}