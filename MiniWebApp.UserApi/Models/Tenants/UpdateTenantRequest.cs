namespace MiniWebApp.UserApi.Models.Tenants;

public record UpdateTenantRequest
{
    public string Name { get; init; } = default!;
    public string? Domain { get; init; }
}
