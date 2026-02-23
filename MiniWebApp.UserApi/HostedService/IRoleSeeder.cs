namespace MiniWebApp.UserApi.HostedService;

public interface IRoleSeeder
{
    Task SeedAsync(CancellationToken ct);
}
