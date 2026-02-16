namespace MiniWebApp.UserApi.Contracts.Tenants;

public record CreateTenantRequest(
    string Name,
    string? Domain
);