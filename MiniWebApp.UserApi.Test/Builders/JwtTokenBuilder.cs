using Microsoft.IdentityModel.Tokens;
using MiniWebApp.Core.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MiniWebApp.UserApi.Test.Builders;

public sealed class JwtTokenBuilder
{
    private string _issuer = "TestIssuer";
    private string _audience = "TestAudience";
    private string _secret = "ThisIsAStrongSecretForTestingPurposeOnly_64BytesLongString!!!";
    private DateTime _expires = DateTime.UtcNow.AddHours(1);
    private List<Claim> _claims = new();

    public static JwtTokenBuilder Create() => new();

    public JwtTokenBuilder ForUser(Guid userId, string email = "test@example.com")
    {
        _claims.Add(new(ClaimTypes.NameIdentifier, userId.ToString()));
        _claims.Add(new(ClaimTypes.Email, email));
        return this;
    }

    public JwtTokenBuilder WithRole(string role)
    {
        _claims.Add(new(ClaimTypes.Role, role));
        return this;
    }

    public JwtTokenBuilder WithPermission(string permission)
    {
        _claims.Add(new(AppClaimTypes.Permissions, permission));
        return this;
    }

    public JwtTokenBuilder ForTenant(Guid tenantId)
    {
        _claims.Add(new(AppClaimTypes.TenantId, tenantId.ToString()));
        return this;
    }

    public JwtTokenBuilder Expired()
    {
        _expires = DateTime.UtcNow.AddMinutes(-30);
        return this;
    }

    public string Build()
    {
        var key = new SymmetricSecurityKey(Convert.FromBase64String(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: _claims,
            expires: _expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
