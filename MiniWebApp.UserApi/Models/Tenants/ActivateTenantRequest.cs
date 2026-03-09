namespace MiniWebApp.UserApi.Models.Tenants;

public record ActivateTenantRequest
{
    public Guid TenantId { get; init; }
}
