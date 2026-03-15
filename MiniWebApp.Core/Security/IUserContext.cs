namespace MiniWebApp.Core.Security;

/// <summary>
/// Provides access to the current authenticated user's information and permissions.
/// </summary>
public interface IUserContext : ITenantProvider
{
    /// <summary>Gets a value indicating whether the current user is authenticated.</summary>
    bool IsAuthenticated { get; }

    /// <summary>Gets a value indicating whether the user has the SuperAdmin role.</summary>
    bool IsSuperAdmin { get; }

    /// <summary>Gets the unique identifier of the user. Throws if not authenticated.</summary>
    Guid UserId { get; }

    /// <summary>Gets the user's email address.</summary>
    string Email { get; }

    /// <summary>Gets the user's display name.</summary>
    string UserName { get; }

    /// <summary>Gets the set of roles assigned to the user.</summary>
    IReadOnlySet<string> Roles { get; }

    /// <summary>Gets the set of granular permissions assigned to the user.</summary>
    IReadOnlySet<string> Permissions { get; }
}