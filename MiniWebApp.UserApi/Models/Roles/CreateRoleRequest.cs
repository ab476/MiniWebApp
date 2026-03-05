namespace MiniWebApp.UserApi.Models.Roles;

public record CreateRoleRequest
{
    public Guid TenantId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
}