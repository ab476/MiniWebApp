namespace MiniWebApp.UserApi.Models.Tenants;

public record TenantResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Domain { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
