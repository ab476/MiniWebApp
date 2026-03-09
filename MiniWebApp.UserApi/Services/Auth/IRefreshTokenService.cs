namespace MiniWebApp.UserApi.Services.Auth;

public interface IRefreshTokenService
{
    Task<Outcome<TokenResponse>> CreateInitialTokensAsync(Guid userId, string ipAddress, CancellationToken ct = default);
    Task<Outcome> RevokeAllUserTokensAsync(Guid userId, CancellationToken ct = default);
    Task<Outcome<TokenResponse>> RotateTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken ct = default);
}


