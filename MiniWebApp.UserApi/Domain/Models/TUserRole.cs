namespace MiniWebApp.UserApi.Domain.Models;

public class TUserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public TUser User { get; set; } = default!;
    public TRole Role { get; set; } = default!;
}
