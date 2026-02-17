namespace MiniWebApp.UserApi.Contracts.Roles;

public record RoleResponse
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string Name { get; init; } = default!;
    public string NormalizedName { get; init; } = default!;
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
}









