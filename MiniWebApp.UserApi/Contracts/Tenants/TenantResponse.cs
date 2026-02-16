namespace MiniWebApp.UserApi.Contracts.Tenants;

public record TenantResponse(
    Guid Id,
    string Name,
    string? Domain,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

