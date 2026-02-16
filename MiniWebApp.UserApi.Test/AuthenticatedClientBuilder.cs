using global::MiniWebApp.Core.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http.Headers;

namespace MiniWebApp.UserApi.Test;

public sealed class AuthenticatedClientBuilder(
    UserApiFactory factory,
    JwtTokenGenerator tokenGenerator)
{
    private readonly UserApiFactory _factory = factory;
    private readonly JwtTokenGenerator _tokenGenerator = tokenGenerator;

    private Guid _userId = Guid.NewGuid();
    private Guid _tenantId = Guid.NewGuid();
    private string _email = string.Empty;
    private string _userName = string.Empty;

    private IEnumerable<string> _permissions = [];
    private IEnumerable<string> _roles = [];

    public AuthenticatedClientBuilder WithUser(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public AuthenticatedClientBuilder WithTenant(Guid tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public AuthenticatedClientBuilder WithPermissions(params string[] permissions)
    {
        _permissions = permissions;
        return this;
    }

    public AuthenticatedClientBuilder WithRoles(params string[] roles)
    {
        _roles = roles;
        return this;
    }

    public HttpClient Build()
    {
        var client = _factory.CreateClient();

        var user = new JwtUser(
            UserId: _userId,
            Email: _email,
            UserName: _userName,
            TenantId: _tenantId,
            Permissions: [.. _permissions],
            Roles: [.. _roles]
        );

        var token = _tokenGenerator.Generate(user);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                token
            );

        return client;
    }
}

