using Microsoft.Extensions.Options;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Domain.Models;
using MiniWebApp.UserApi.Options;

namespace MiniWebApp.UserApi.HostedService;

public interface ITenantSeeder
{
    Task SeedAsync(CancellationToken ct);
}
