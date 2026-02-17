namespace MiniWebApp.UserApi.Contracts.Tenants;

public record CreateTenantRequest(
    string Name,
    string? Domain
);
public record ActivateTenantRequest(Guid TenantId);

public record DeactivateTenantRequest(Guid TenantId);

public record TenantResponse(
    Guid Id,
    string Name,
    string? Domain,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record UpdateTenantRequest(
    string Name,
    string? Domain
);
