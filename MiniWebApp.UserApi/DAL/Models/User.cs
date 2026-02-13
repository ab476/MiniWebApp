namespace MiniWebApp.UserApi.DAL.Models;

public class User
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string Email { get; set; } = default!;
    public string NormalizedEmail { get; set; } = default!;

    public string Username { get; set; } = default!;
    public string NormalizedUsername { get; set; } = default!;

    public string PasswordHash { get; set; } = default!;
    public string SecurityStamp { get; set; } = default!;

    public short Status { get; set; }

    public int FailedLoginAttempts { get; set; }

    public DateTime? LockoutEnd { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public DateTime RowVersion { get; set; }

    public Tenant Tenant { get; set; } = default!;

    public ICollection<UserRole> UserRoles { get; set; } = [];
}
