using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using MiniWebApp.Core.Auth;
using MiniWebApp.Core.Services; // For IScopedStateService
using System.Security.Claims;
using UnauthorizedAccessException = MiniWebApp.Core.Exceptions.UnauthorizedAccessException;

namespace MiniWebApp.Core.Security;

/// <inheritdoc cref="IUserContext"/>
public sealed class UserContext(
    IHttpContextAccessor httpContextAccessor,
    IScopedStateService stateService) : IUserContext, ITenantProvider
{
    // --- State Resolution ---

    /// <summary>
    /// Attempts to resolve user data from Scoped State if HttpContext is unavailable.
    /// </summary>
    private JwtUser? ScopedUser => stateService.Get<JwtUser>();

    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    // --- Cached Backing Fields ---
    private Guid? _userId;
    private Guid? _tenantId;
    private string? _email;
    private string? _userName;
    private bool? _isSuperAdmin;
    private IReadOnlySet<string>? _roles;
    private IReadOnlySet<string>? _permissions;

    // --- Implementation ---

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated
                                   ?? ScopedUser is not null;

    public bool IsSuperAdmin => _isSuperAdmin ??= IsInRole(AppRoles.SuperAdmin);

    public Guid UserId => _userId ??= ResolveGuid(
        claimType: ClaimTypes.NameIdentifier,
        scopedValue: ScopedUser?.UserId,
        displayName: "User ID");

    public Guid TenantId => _tenantId ??= ResolveGuid(
        claimType: AppClaimTypes.TenantId,
        scopedValue: ScopedUser?.TenantId,
        displayName: "Tenant ID");

    public string Email => _email ??= Principal?.FindFirstValue(ClaimTypes.Email)
                                      ?? ScopedUser?.Email
                                      ?? throw new UnauthorizedAccessException("User identification (Email) is missing.");

    public string UserName => _userName ??= Principal?.FindFirstValue(JwtRegisteredClaimNames.UniqueName)
                                            ?? ScopedUser?.UserName
                                            ?? string.Empty;

    public IReadOnlySet<string> Roles => _roles ??= ResolveSet(ClaimTypes.Role, ScopedUser?.Roles);

    public IReadOnlySet<string> Permissions => _permissions ??= ResolveSet(AppClaimTypes.Permissions, ScopedUser?.Permissions);

    // --- Private Helpers ---

    private bool IsInRole(string role)
    {
        if (Principal is not null) return Principal.IsInRole(role);
        return ScopedUser?.Roles?.Contains(role, StringComparer.OrdinalIgnoreCase) ?? false;
    }

    private Guid ResolveGuid(string claimType, Guid? scopedValue, string displayName)
    {
        // 1. Try HttpContext
        var claimValue = Principal?.FindFirstValue(claimType);
        if (Guid.TryParse(claimValue, out var result) && result != Guid.Empty)
            return result;

        // 2. Try Scoped State
        if (scopedValue.HasValue && scopedValue != Guid.Empty)
            return scopedValue.Value;

        throw new UnauthorizedAccessException($"{displayName} is missing from both HttpContext and Scoped State.");
    }

    private HashSet<string> ResolveSet(string claimType, IEnumerable<string>? scopedList)
    {
        // Priority 1: Claims from HttpContext
        if (Principal is not null)
        {
            var claims = Principal.FindAll(claimType)
                .Select(c => c.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (claims.Count > 0) return claims;
        }

        // Priority 2: Scoped State
        return scopedList?.ToHashSet(StringComparer.OrdinalIgnoreCase)
               ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
}