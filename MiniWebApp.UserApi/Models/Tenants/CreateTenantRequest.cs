namespace MiniWebApp.UserApi.Models.Tenants;

public record CreateTenantRequest
{
    public string Name { get; init; } = null!;
    public string? Domain { get; init; }
}
