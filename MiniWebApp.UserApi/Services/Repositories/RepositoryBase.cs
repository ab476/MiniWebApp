using MiniWebApp.Core.Security;

namespace MiniWebApp.UserApi.Services.Repositories;

/// <summary>
/// Provides a base class for repositories, offering shared access to operational context 
/// such as multi-tenancy data, current user information, and system time.
/// </summary>
/// <param name="context">The ambient request context for the current request.</param>
public abstract class RepositoryBase(IRequestContext context)
{
    /// <summary>Gets the unique identifier of the tenant associated with the current request context.</summary>
    protected Guid ContextTenantId => context.TenantId;

    /// <summary>Gets the current coordinated universal time (UTC) provided by the system time provider.</summary>
    protected DateTime UtcNow => context.UtcNow;

    /// <summary>Gets a value indicating whether the current authenticated user has administrative privileges across all tenants.</summary>
    protected bool IsSuperAdmin => context.IsSuperAdmin;

    /// <summary>Gets the unique identifier of the currently authenticated user, or <see langword="null"/> if the request is anonymous.</summary>
    protected Guid? UserId => context.UserId;
}

/// <summary>
/// Defines the ambient context for the current request, aggregating user security, 
/// multi-tenancy, and environmental data.
/// </summary>
public interface IRequestContext
{
    /// <summary>Gets the current authenticated user context.</summary>
    IUserContext User { get; }

    /// <summary>Gets the unique identifier of the currently authenticated user, or <see langword="null"/> if the request is anonymous.</summary>
    Guid? UserId { get; }

    /// <summary>Gets the provider used to retrieve system time.</summary>
    TimeProvider TimeProvider { get; }

    /// <summary>Gets the tenant identifier resolved for the current scope.</summary>
    Guid TenantId { get; }

    /// <summary>Gets the current date and time in UTC.</summary>
    DateTime UtcNow { get; }

    /// <summary>Gets a value indicating whether the current user is a super administrator.</summary>
    bool IsSuperAdmin { get; }
}

/// <summary>
/// Default implementation of <see cref="IRequestContext"/> that resolves values 
/// from the injected user context and time provider.
/// </summary>
/// <param name="userContext">The authenticated user context.</param>
/// <param name="timeProvider">The system time provider.</param>
public class OperationalContext(IUserContext userContext, TimeProvider timeProvider) : IRequestContext
{
    /// <inheritdoc />
    public IUserContext User { get; } = userContext;

    /// <inheritdoc />
    public TimeProvider TimeProvider { get; } = timeProvider;

    /// <inheritdoc />
    public Guid TenantId => User.TenantId;

    /// <inheritdoc />
    public DateTime UtcNow => TimeProvider.GetUtcNow().UtcDateTime;

    /// <inheritdoc />
    public bool IsSuperAdmin => User.IsSuperAdmin;

    public Guid? UserId => User.IsAuthenticated ? User.UserId : null;
}