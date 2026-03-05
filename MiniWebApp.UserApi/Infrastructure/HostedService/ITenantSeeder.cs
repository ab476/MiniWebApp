namespace MiniWebApp.UserApi.Infrastructure.HostedService;

public interface ITenantSeeder
{
    Task SeedAsync(CancellationToken ct);
}
