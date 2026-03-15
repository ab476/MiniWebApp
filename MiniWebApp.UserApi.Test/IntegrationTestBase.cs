using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MiniWebApp.Core.Security;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Test.Builders;
using MiniWebApp.UserApi.Test.Builders.Tenants;

namespace MiniWebApp.UserApi.Test;



[Collection("ApiCollection")]
public abstract class IntegrationTestBase : IDisposable
{
    private readonly IOptions<JwtOptions> _options = new TestJwtSettings();

    protected readonly JwtTokenGenerator JwtTokenGenerator;
    protected readonly UserApiFactory Factory;
    protected readonly IServiceScope Scope;
    protected IntegrationTestBase(PostgresContainerFixture fixture)
    {
        JwtTokenGenerator = new JwtTokenGenerator(_options);
        Factory = new UserApiFactory(fixture.ConnectionString, _options);
        Scope = Factory.Services.CreateScope();
    }
    protected static CancellationToken CancellationToken => TestContext.Current.CancellationToken;
    protected static CancellationToken ct => CancellationToken;

    // ----------------------------
    // Client Creation (Centralized)
    // ----------------------------

    protected HttpClient GetAnonymousClient()
    {
        return Factory.CreateClient();
    }

    protected AuthenticatedClientBuilder AuthClientBuilder()
    {
        return new AuthenticatedClientBuilder(
            Factory,
            JwtTokenGenerator
        );
    }

    protected async Task<Guid> CreateRoleAsync()
    {
        throw new NotImplementedException();
    }

    protected async Task<Guid> CreateTenantWithRolesAsync()
    {
        throw new NotImplementedException();
    }

    protected async Task<Guid> CreateTenantAsync()
    {
        throw new NotImplementedException();
    }
    protected T GetService<T>() where T : notnull
    {
        return Scope.ServiceProvider.GetRequiredService<T>();
    }

    public void Dispose()
    {
        Scope.Dispose();
    }

    protected UserDbContext DbContext => GetService<UserDbContext>();

    protected async Task<Tenant> SeedTenantAsync(Action<TenantBuilder>? configure = null)
    {
        return (await SeedTenantsAsync(configure ?? (static _ => { })))[0];
    }

    protected async Task<List<Tenant>> SeedTenantsAsync(params Action<TenantBuilder>[] configurations)
    {
        var tenants = configurations.Select(configure =>
        {
            var builder = TenantBuilder.Default;
            configure(builder);
            return builder.Build();
        }).ToList();

        await DbContext.Tenants.AddRangeAsync(tenants, CancellationToken);
        await DbContext.SaveChangesAsync(CancellationToken);
        DbContext.ChangeTracker.Clear();

        return tenants;
    }
    #region Role Helpers

    protected Guid? CachedTenantId { get; set; }

    protected async Task<Role> SeedRoleAsync(Action<RoleBuilder>? configure = null)
    {
        var roles = await SeedRolesAsync(configure ??= (_ => { }));
        return roles[0];
    }

    protected async Task<List<Role>> SeedRolesAsync(params Action<RoleBuilder>[] configurations)
    {
        // 1. Ensure a parent Tenant exists for the Foreign Key constraint
        if (CachedTenantId is null)
        {
            var tenant = await SeedTenantAsync();
            CachedTenantId = tenant.Id;
        }

        var roles = configurations.Select(configure =>
        {
            var builder = RoleBuilder.Default;
            configure(builder);
            var role = builder.Build();

            if (role.TenantId == Guid.Empty)
            {
                role.TenantId = CachedTenantId.Value;
            }

            return role;
        }).ToList();

        // 3. Persist all roles in one batch
        await DbContext.Roles.AddRangeAsync(roles, CancellationToken);
        await DbContext.SaveChangesAsync(CancellationToken);

        // 4. Clear tracker so 'Act' phase queries the database, not the cache
        DbContext.ChangeTracker.Clear();

        return roles;
    }
    protected Task<List<Role>> SeedRolesAsync(int count, Action<RoleBuilder, int>? configure = null)
    {
        Action<RoleBuilder>[] actions = [..
            Enumerable
                .Range(0, count)
                .Select(i => (Action<RoleBuilder>)(b => configure?.Invoke(b, i)))
        ];
        return SeedRolesAsync(actions);
    }

    #endregion
}
