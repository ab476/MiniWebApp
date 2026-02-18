namespace MiniWebApp.UserApi.Domain.Models;

public class TPermission
{
    public Guid Id { get; set; }

    public string Code { get; set; } = default!;

    public string? Description { get; set; }

    public string? Category { get; set; }
    public ICollection<TRolePermission> RolePermissions { get; set; } = [];
}
