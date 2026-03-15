using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using MiniWebApp.Core.Auth;

namespace MiniWebApp.Core.Security;

/// <inheritdoc cref="IJwtTokenGenerator"/>
public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private static readonly JsonWebTokenHandler _handler = new();
    private readonly JwtOptions _options;
    private readonly SigningCredentials _credentials;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtTokenGenerator"/> with injected options.
    /// </summary>
    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        _options = options.Value;

        var securityKey = new SymmetricSecurityKey(_options.GetBytes());

        _credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);
    }

    /// <inheritdoc />
    public string Generate(JwtUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        // Pre-calculate capacity for performance: 5 standard + roles + permissions
        int capacity = 6 + (user.Roles?.Count ?? 0) + (user.Permissions?.Count ?? 0);

        var claims = new List<Claim>(capacity)
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(AppClaimTypes.TenantId, user.TenantId?.ToString() ?? string.Empty)
        };

        if (user.Roles is not null)
        {
            foreach (var role in user.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (user.Permissions is not null)
        {
            foreach (var permission in user.Permissions)
                claims.Add(new Claim(AppClaimTypes.Permissions, permission));
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
            SigningCredentials = _credentials
        };

        return _handler.CreateToken(descriptor);
    }
}