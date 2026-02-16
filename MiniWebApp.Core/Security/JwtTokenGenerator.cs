using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MiniWebApp.Core.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace MiniWebApp.Core.Security;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private static readonly JwtSecurityTokenHandler _handler = new();

    private readonly JwtOptions _options;
    private readonly SigningCredentials _credentials;

    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        _options = options.Value;

        var keyBytes = _options.GetBytes();

        var securityKey = new SymmetricSecurityKey(keyBytes);

        _credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);
    }

    public string Generate(JwtUser user)
    {
        var (userId, email, userName, tenantId, roles, permissions) = user;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.UniqueName, userName ?? string.Empty),
            new(AppClaimTypes.TenantId, tenantId?.ToString() ?? string.Empty)
        };

        if (roles is not null)
        {
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (permissions is not null)
        {
            foreach (var permission in permissions)
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

        var token = _handler.CreateToken(descriptor);

        return _handler.WriteToken(token);
    }
}
public sealed record JwtUser(
    Guid UserId,
    string Email,
    string? UserName,
    Guid? TenantId,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions
);