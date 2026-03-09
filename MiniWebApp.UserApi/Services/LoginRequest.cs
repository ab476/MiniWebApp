using System.Net;

namespace MiniWebApp.UserApi.Services;

public record LoginRequest(
    string Email,
    string Password,
    string? DeviceInfo = null,
    IPAddress? IpAddress = null
)
{
    public Guid TenantId { get; internal set; }
}