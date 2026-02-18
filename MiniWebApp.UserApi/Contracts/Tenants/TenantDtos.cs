namespace MiniWebApp.UserApi.Contracts.Tenants;

public record CreateTenantRequest
{
    public string Name { get; init; } = null!;
    public string? Domain { get; init; }
}

public record ActivateTenantRequest
{
    public Guid TenantId { get; init; }
}


public record DeactivateTenantRequest
{
    public Guid TenantId { get; init; }
}


public record TenantResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Domain { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}


public record UpdateTenantRequest
{
    public string Name { get; init; } = default!;
    public string? Domain { get; init; }
}
