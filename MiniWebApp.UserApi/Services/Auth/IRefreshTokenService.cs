using System.Net;

namespace MiniWebApp.UserApi.Services.Auth;

public interface IRefreshTokenService
{
    Task<Outcome<RefreshTokenResponse>> CreateInitialTokensAsync(Guid userId, IPAddress? ipAddress, CancellationToken ct = default);
    Task<Outcome<RefreshTokenResponse>> RotateAsync(string refreshToken, IPAddress? ipAddress, CancellationToken ct = default);
    Task<Outcome<IReadOnlyList<RefreshTokenResponse>>> GetActiveTokensByUserAsync(Guid userId, CancellationToken ct = default);
    Task<Outcome> RevokeAllForUserAsync(Guid userId, CancellationToken ct = default);
    Task<Outcome> PurgeExpiredTokensAsync(CancellationToken ct = default);
}
