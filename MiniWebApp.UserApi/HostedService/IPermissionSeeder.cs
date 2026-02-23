namespace MiniWebApp.UserApi.HostedService;

public interface IPermissionSeeder
{
    Task SeedAsync(CancellationToken ct);
}
