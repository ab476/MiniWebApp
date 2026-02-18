namespace MiniWebApp.UserApi.Domain.Models;

public class TRole
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string Name { get; set; } = default!;
    public string NormalizedName { get; set; } = default!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public TTenant Tenant { get; set; } = default!;
    public ICollection<TRolePermission> RolePermissions { get; set; } = [];
    public ICollection<TUserRole> UserRoles { get; set; } = [];
}
