namespace MiniWebApp.UserApi.Infrastructure.HostedService;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken ct);
}
