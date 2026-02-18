namespace MiniWebApp.UserApi.Models.Tenants;

public record DeactivateTenantRequest
{
    public Guid TenantId { get; init; }
}
