namespace MiniWebApp.UserApi.Domain.Models;

public class TRolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    public TRole Role { get; set; } = default!;
    public TPermission Permission { get; set; } = default!;
}
