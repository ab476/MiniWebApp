namespace MiniWebApp.UserApi.Infrastructure.HostedService;

public interface IUserSeeder
{
    Task SeedAsync(CancellationToken ct);
}
