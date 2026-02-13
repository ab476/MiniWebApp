namespace MiniWebApp.UserApi.DAL.Models;

public class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    public Role Role { get; set; } = default!;
    public Permission Permission { get; set; } = default!;
}
