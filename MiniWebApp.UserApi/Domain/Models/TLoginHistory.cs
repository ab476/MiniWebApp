namespace MiniWebApp.UserApi.Domain.Models;

public class TLoginHistory
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }

    public DateTime LoginTime { get; set; }

    public string? IpAddress { get; set; }
    public string? DeviceInfo { get; set; }
    public string? Location { get; set; }

    public bool IsSuccessful { get; set; }

    public TUser User { get; set; } = default!;
    public TTenant Tenant { get; set; } = default!;
}
