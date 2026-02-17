namespace MiniWebApp.UserApi.Contracts.Roles;

public record CreateRoleRequest
{
    public Guid TenantId { get; init; }
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
}