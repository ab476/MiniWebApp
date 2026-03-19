using System.Net;

namespace MiniWebApp.UserApi.Services.Auth;

public interface IAuthService
{
    Task<Outcome<LoginResponse>> LoginAsync(LoginRequest request, IPAddress? ipAddress, string? deviceInfo, CancellationToken ct = default);
    Task<Outcome> LogoutAsync(Guid userId, CancellationToken ct = default);
    Task<Outcome<LoginResponse>> RefreshTokenAsync(string refreshToken, IPAddress? ipAddress, CancellationToken ct = default);
}