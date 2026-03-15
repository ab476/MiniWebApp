namespace MiniWebApp.UserApi.Domain.Models;

public class UserRole
{
    public Guid UserId { get; set; }
    public required string RoleCode { get; set; }
    public Guid TenantId { get; set; }
    public User User { get; set; } = default!;
    public Role Role { get; set; } = default!;
}
