namespace MiniWebApp.UserApi.HostedService;

public interface IUserSeeder
{
    Task SeedAsync(CancellationToken ct);
}
