namespace MiniWebApp.UserApi.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    public required string Email
    {
        get;
        set;
    }
    public required string NormalizedEmail { get; set; }

    public required string UserName
    {
        get;
        set;
    }
    public required string NormalizedUsername { get; set; }

    public required string PasswordHash { get; set; }
    public bool EmailConfirmed { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Pending;

    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    // Navigation
    public Tenant Tenant { get; set; } = default!;
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
public enum UserStatus { Pending = 0, Active = 1, Suspended = 2, Deleted = 3 }