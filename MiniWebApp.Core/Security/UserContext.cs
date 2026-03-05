using Microsoft.AspNetCore.Http;
using MiniWebApp.Core.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InvalidOperationException = MiniWebApp.Core.Exceptions.InvalidOperationException;
using UnauthorizedAccessException = MiniWebApp.Core.Exceptions.UnauthorizedAccessException;

namespace MiniWebApp.Core.Security;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private HttpContext Context => httpContextAccessor.HttpContext
        ?? throw new InvalidOperationException("HTTP Context is unavailable.");

    private ClaimsPrincipal Principal => Context.User;

    // --- Cached Backing Fields ---
    private Guid? _userId;
    private Guid? _tenantId;
    private string? _email;
    private string? _userName;
    private bool? _isSuperAdmin;
    private IReadOnlySet<string>? _roles;
    private IReadOnlySet<string>? _permissions;

    // --- Implementation ---

    public bool IsAuthenticated => Principal.Identity?.IsAuthenticated ?? false;

    public bool IsSuperAdmin => _isSuperAdmin ??= Principal.IsInRole(AppRoles.SuperAdmin);

    public Guid UserId => _userId ??= GetRequiredGuid(ClaimTypes.NameIdentifier, "User ID");

    public Guid TenantId => _tenantId ??= GetRequiredGuid(AppClaimTypes.TenantId, "Tenant ID");

    public string Email => _email ??= Principal.FindFirstValue(ClaimTypes.Email)
        ?? throw new UnauthorizedAccessException("Email claim is missing.");

    public string UserName => _userName ??= Principal.FindFirstValue(JwtRegisteredClaimNames.UniqueName) ?? string.Empty;

    public IReadOnlySet<string> Roles => _roles ??= GetClaimsList(ClaimTypes.Role);

    public IReadOnlySet<string> Permissions => _permissions ??= GetClaimsList(AppClaimTypes.Permissions);

    // --- Helper Methods ---

    private Guid GetRequiredGuid(string claimType, string displayName)
    {
        var value = Principal.FindFirstValue(claimType);
        if (!Guid.TryParse(value, out var result) || result == Guid.Empty)
        {
            throw new UnauthorizedAccessException($"{displayName} claim is missing or invalid.");
        }
        return result;
    }

    private HashSet<string> GetClaimsList(string claimType)
    {
        return Principal.FindAll(claimType)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.InvariantCultureIgnoreCase);
    }
}