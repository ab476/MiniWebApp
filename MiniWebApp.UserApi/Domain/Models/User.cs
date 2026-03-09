namespace MiniWebApp.UserApi.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    public string Email
    {
        get;
        set
        {
            field = value;
            NormalizedEmail = value.ToUpperInvariant();
        }
    } = default!;
    public string NormalizedEmail { get; private set; } = default!;

    public string UserName
    {
        get;
        set
        {
            field = value;
            NormalizedUsername = value.ToUpperInvariant();
        }
    } = default!;
    public string NormalizedUsername { get; private set; } = default!;

    public string PasswordHash { get; set; } = default!;
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