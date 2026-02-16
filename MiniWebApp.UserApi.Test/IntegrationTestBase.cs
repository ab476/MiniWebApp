using Microsoft.Extensions.Options;
using MiniWebApp.Core.Security;

namespace MiniWebApp.UserApi.Test;



[Collection("ApiCollection")]
public abstract class IntegrationTestBase
{
    private readonly IOptions<JwtOptions> _options = new TestJwtSettings();

    protected readonly JwtTokenGenerator JwtTokenGenerator;
    protected readonly UserApiFactory Factory;

    protected IntegrationTestBase(PostgresContainerFixture fixture)
    {
        JwtTokenGenerator = new JwtTokenGenerator(_options);
        Factory = new UserApiFactory(fixture.ConnectionString, _options);
    }

    // ----------------------------
    // Client Creation (Centralized)
    // ----------------------------

    protected HttpClient CreateAnonymousClient()
    {
        return Factory.CreateClient();
    }

    protected AuthenticatedClientBuilder CreateAuthenticatedClient()
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

}
