namespace MiniWebApp.UserApi.Infrastructure.HostedService;

public interface IRoleSeeder
{
    Task SeedAsync(CancellationToken ct);
}
