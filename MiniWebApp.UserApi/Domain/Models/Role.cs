namespace MiniWebApp.UserApi.Domain.Models;

public class Role
{
    public Guid TenantId { get; set; }
    public string RoleCode { get; set; } = default!;
    public string? DisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }   
    public Tenant Tenant { get; set; } = default!;
    public ICollection<RoleClaim> RoleClaims { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
