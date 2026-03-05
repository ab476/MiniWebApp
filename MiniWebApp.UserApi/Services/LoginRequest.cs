namespace MiniWebApp.UserApi.Services;

public record LoginRequest(
    string Email,
    string Password,
    string? DeviceInfo = null,
    string? IpAddress = null
)
{
    public Guid TenantId { get; internal set; }
}