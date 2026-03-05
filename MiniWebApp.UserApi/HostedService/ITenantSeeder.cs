namespace MiniWebApp.UserApi.HostedService;

public interface ITenantSeeder
{
    Task SeedAsync(CancellationToken ct);
}
