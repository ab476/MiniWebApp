namespace MiniWebApp.UserApi.Contracts.Tenants;

public record UpdateTenantRequest(
    string Name,
    string? Domain);

