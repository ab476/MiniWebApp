namespace MiniWebApp.UserApi.Domain.Models;

public class LoginHistory
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }

    public DateTime LoginTime { get; set; }

    public string? IpAddress { get; set; }
    public string? DeviceInfo { get; set; }
    public string? Location { get; set; }

    public bool IsSuccessful { get; set; }

    public User User { get; set; } = default!;
    public Tenant Tenant { get; set; } = default!;
}
