namespace MiniWebApp.Core.Security;

/// <summary>
/// Defines a contract for accessing the current tenant's identity.
/// <br/>
/// This is primarily used for multi-tenant data isolation and resource scoping.
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// Gets the unique identifier of the tenant associated with the current request context.
    /// <br/>
    /// <br/>
    /// <b>Exceptions:</b>
    /// <list type="bullet">
    /// <item>
    /// <description>Throws an <c>UnauthorizedAccessException</c> if the tenant claim is missing or invalid.</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <value>A <see cref="Guid"/> representing the current tenant.</value>
    Guid TenantId { get; }
}