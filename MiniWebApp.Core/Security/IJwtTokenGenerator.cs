namespace MiniWebApp.Core.Security;

/// <summary>
/// Defines a contract for generating cryptographically signed JSON Web Tokens (JWT).
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a signed JWT string for a specific user containing their identity, roles, and permissions.
    /// <br/>
    /// <br/>
    /// <b>Generated Claims:</b>
    /// <list type="bullet">
    /// <item><description><c>sub</c>: The unique User ID (Subject).</description></item>
    /// <item><description><c>jti</c>: A unique Token ID for tracking and revocation.</description></item>
    /// <item><description><c>role</c>: One or more user roles for RBAC.</description></item>
    /// <item><description><c>permissions</c>: Granular application-specific strings for PBAC.</description></item>
    /// </list>
    /// </summary>
    /// <param name="user">The <see cref="JwtUser"/> identity data.</param>
    /// <returns>A signed JWT string.</returns>
    string Generate(JwtUser user);
}


/// <summary>
/// Represents the user identity and authorization data required to generate a JWT.
/// </summary>
/// <param name="UserId">The unique identifier for the user (Subject).</param>
/// <param name="Email">The user's primary email address.</param>
/// <param name="UserName">The user's display name or handle.</param>
/// <param name="TenantId">Optional identifier for multi-tenant isolation.</param>
/// <param name="Roles">A collection of logical roles (e.g., "Admin", "User").</param>
/// <param name="Permissions">A collection of granular action-based strings (e.g., "users:read").</param>
public sealed record JwtUser(
    Guid UserId,
    string Email,
    string? UserName,
    Guid? TenantId,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);