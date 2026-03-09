using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MiniWebApp.Core.Security;

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
}
