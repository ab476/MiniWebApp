using MiniWebApp.Core.Security;
using MiniWebApp.Core.Services;
using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Infrastructure.HostedService;

/// <summary>
/// Provides a mechanism to establish a system-level identity context, typically used 
/// during background tasks or automated operations like database seeding.
/// </summary>
public interface ISystemIdentitySetupService
{
    /// <summary>
    /// Configures the current execution context with a system administrator identity.
    /// </summary>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous setup operation.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the required 'System' or 'Default' tenant cannot be located in the database.
    /// </exception>
    Task SetupSystemContextAsync(CancellationToken ct = default);
}

/// <summary>
/// Default implementation of <see cref="ISystemIdentitySetupService"/> that constructs a highly privileged 
/// <see cref="JwtUser"/> and injects it into the application's scoped state.
/// </summary>
/// <param name="dbContext">The database context used to resolve the system tenant identifier.</param>
/// <param name="scopedState">The state service where the generated system identity will be stored.</param>
public class SystemIdentitySetupService(
    UserDbContext dbContext,
    IScopedStateService scopedState) : ISystemIdentitySetupService
{
    /// <inheritdoc />
    public async Task SetupSystemContextAsync(CancellationToken ct = default)
    {
        var tenant = await dbContext.Set<Tenant>()
            .FirstOrDefaultAsync(t => t.Name == "System" || t.Name == "Default", ct)
            ?? throw new InvalidOperationException("System Tenant not found.");

        scopedState.Set(new JwtUser(
            UserId: Guid.Empty,
            Email: "system@internal",
            TenantId: tenant.Id,
            UserName: "System_Orchestrator",
            Roles: ["SuperAdmin"],
            Permissions: ["*"]
        ));
    }
}