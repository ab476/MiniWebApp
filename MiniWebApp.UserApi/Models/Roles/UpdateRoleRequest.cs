namespace MiniWebApp.UserApi.Models.Roles;

public record UpdateRoleRequest
{
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
}









