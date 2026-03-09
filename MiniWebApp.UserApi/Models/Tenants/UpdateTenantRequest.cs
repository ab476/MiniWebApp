namespace MiniWebApp.UserApi.Models.Tenants;

public record UpdateTenantRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Domain { get; init; }
}
