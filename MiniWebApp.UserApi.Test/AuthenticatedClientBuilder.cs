using global::MiniWebApp.Core.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MiniWebApp.Core.Auth;
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

    private List<string> _permissions = [];
    private List<string> _roles = [];

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
        _permissions.AddRange(permissions);
        return this;
    }

    public AuthenticatedClientBuilder WithRoles(params string[] roles)
    {
        _roles.AddRange(roles);
        return this;
    }
    // Fluent Accessors for specific modules
    public TenantPermissionBuilder Tenants => new(this);
    public RolePermissionBuilder Roles => new(this);
    public UserPermissionBuilder Users => new(this);
    public SystemPermissionsBuilder SystemPermissions => new(this); 

    // Internal helper to add a single permission and return the main builder
    internal AuthenticatedClientBuilder AddPermission(string permission)
    {
        _permissions.Add(permission);
        return this;
    }
    public AuthenticatedClientBuilder AsSuperAdmin()
    {
        _permissions.Clear();
        _permissions.AddRange(AppPermissions.All);
        _roles.Clear();
        _roles.Add(AppRoles.SuperAdmin);
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
    

    public sealed class TenantPermissionBuilder(AuthenticatedClientBuilder parent)
    {
        public AuthenticatedClientBuilder CanRead() => parent.AddPermission(AppPermissions.Tenants.Read);
        public AuthenticatedClientBuilder CanWrite() => parent.AddPermission(AppPermissions.Tenants.Write);
        public AuthenticatedClientBuilder CanManage() => parent.AddPermission(AppPermissions.Tenants.Manage);
        public AuthenticatedClientBuilder FullControl() => parent.WithPermissions([.. AppPermissions.Tenants.All]);
    }
    public sealed class SystemPermissionsBuilder(AuthenticatedClientBuilder parent)
    {
        public AuthenticatedClientBuilder CanRead() => parent.AddPermission(AppPermissions.Permissions.Read);
    }
    public sealed class RolePermissionBuilder(AuthenticatedClientBuilder parent)
    {
        public AuthenticatedClientBuilder CanRead() => parent.AddPermission(AppPermissions.Roles.Read);
        public AuthenticatedClientBuilder CanWrite() => parent.AddPermission(AppPermissions.Roles.Write);
        public AuthenticatedClientBuilder CanManage() => parent.AddPermission(AppPermissions.Roles.Manage);
        public AuthenticatedClientBuilder FullControl() => parent.WithPermissions([.. AppPermissions.Roles.All]);
    }

    public sealed class UserPermissionBuilder(AuthenticatedClientBuilder parent)
    {
        public AuthenticatedClientBuilder CanRead() => parent.AddPermission(AppPermissions.Users.Read);
        public AuthenticatedClientBuilder CanWrite() => parent.AddPermission(AppPermissions.Users.Write);
        public AuthenticatedClientBuilder CanManage() => parent.AddPermission(AppPermissions.Users.Manage);
        public AuthenticatedClientBuilder FullControl() => parent.WithPermissions([.. AppPermissions.Users.All]);
    }
}

