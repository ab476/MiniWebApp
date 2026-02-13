namespace MiniWebApp.UserApi.DAL.Models;

public class Role
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string Name { get; set; } = default!;
    public string NormalizedName { get; set; } = default!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public Tenant Tenant { get; set; } = default!;
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
