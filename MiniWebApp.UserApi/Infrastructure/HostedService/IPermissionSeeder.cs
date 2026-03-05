namespace MiniWebApp.UserApi.Infrastructure.HostedService;

public interface IPermissionSeeder
{
    Task SeedAsync(CancellationToken ct);
}
