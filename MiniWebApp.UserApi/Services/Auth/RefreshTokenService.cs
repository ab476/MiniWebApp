using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Auth;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;


public class RefreshTokenService(
    UserDbContext context,
    ILogger<RefreshTokenService> logger,
    TimeProvider timeProvider) : IRefreshTokenService
{
    private const int TokenByteLength = 64;
    private const int ExpiryDays = 7;
    private DbSet<RefreshToken> RefreshTokens => context.RefreshTokens;

    /// <summary>
    /// Creates a new token pair for a user after a successful initial login.
    /// </summary>
    public async Task<Outcome<TokenResponse>> CreateInitialTokensAsync(Guid userId, string ipAddress, CancellationToken ct = default)
    {
        var rawRefreshToken = GenerateSecureRandomString();
        var hashedToken = ComputeHash(rawRefreshToken);
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = hashedToken,
            CreatedAt = now,
            ExpiresAt = now.AddDays(ExpiryDays),
            CreatedByIp = ipAddress
        };

        RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync(ct);

        logger.LogInformation("Initial refresh token created for User: {UserId} from IP: {IpAddress}", userId, ipAddress);

        return (StatusCodes.Status201Created, new TokenResponse(
            RefreshToken: rawRefreshToken,
            RefreshTokenExpiration: refreshToken.ExpiresAt
        ));
    }

    /// <summary>
    /// Rotates an existing token. Implements reuse detection to prevent replay attacks.
    /// </summary>
    public async Task<Outcome<TokenResponse>> RotateTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken ct = default)
    {
        var incomingHash = ComputeHash(request.RefreshToken);
        var now = timeProvider.GetUtcNow().UtcDateTime;

        // Use an execution strategy for resilient connections (SQL Azure, etc.)
        var strategy = context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync<Outcome<TokenResponse>>(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(ct);

            try
            {
                var storedToken = await RefreshTokens
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.TokenHash == incomingHash, ct);

                // 1. Check if token exists or is expired
                if (storedToken == null || storedToken.ExpiresAt < now)
                {
                    logger.LogWarning("Refresh token rotation failed: Token not found or expired. IP: {IpAddress}", ipAddress);
                    return ("Invalid credentials", StatusCodes.Status401Unauthorized);
                }

                // 2. REUSE DETECTION: If token is already revoked, it might be a breach
                if (storedToken.RevokedAt != null)
                {
                    logger.LogCritical("SECURITY ALERT: Refresh token reuse detected! Revoking all sessions for User: {UserId}. IP: {IpAddress}",
                        storedToken.UserId, ipAddress);

                    await RevokeAllUserTokensInternalAsync(storedToken.UserId, ct);
                    await context.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    return ("Invalid credentials", StatusCodes.Status401Unauthorized);
                }

                // 3. Generate new token pair
                var newRawRefreshToken = GenerateSecureRandomString();
                var newToken = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = storedToken.UserId,
                    TokenHash = ComputeHash(newRawRefreshToken),
                    CreatedAt = now,
                    ExpiresAt = now.AddDays(ExpiryDays),
                    CreatedByIp = ipAddress
                };

                // 4. Update old token state
                storedToken.RevokedAt = now;
                storedToken.ReplacedByTokenId = newToken.Id;

                RefreshTokens.Add(newToken);
                // Note: No need for .Update() as EF tracks changes to storedToken automatically

                await context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                logger.LogInformation("Token rotated successfully for User: {UserId}", storedToken.UserId);

                return (StatusCodes.Status200OK, new TokenResponse(
                    RefreshToken: newRawRefreshToken,
                    RefreshTokenExpiration: newToken.ExpiresAt
                ));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during token rotation for IP: {IpAddress}", ipAddress);
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }

    public async Task<Outcome> RevokeAllUserTokensAsync(Guid userId, CancellationToken ct = default)
    {
        var affectedRows = await RevokeAllUserTokensInternalAsync(userId, ct);

        if (affectedRows > 0)
        {
            logger.LogInformation("Manually revoked {Count} active sessions for User: {UserId}", affectedRows, userId);
            return StatusCodes.Status200OK;
        }

        return ("No active sessions found.", StatusCodes.Status404NotFound);
    }

    private async Task<int> RevokeAllUserTokensInternalAsync(Guid userId, CancellationToken ct)
    {
        return await RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.RevokedAt, timeProvider.GetUtcNow().UtcDateTime), ct);
    }

    private static byte[] ComputeHash(string input)
    {
        return SHA256.HashData(Encoding.UTF8.GetBytes(input));
    }

    private static string GenerateSecureRandomString()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(TokenByteLength));
    }
}

/// <summary>
/// The response sent to the client containing the JWT and the Refresh Token.
/// </summary>
public record TokenResponse(
    string RefreshToken,
    DateTime RefreshTokenExpiration
);

/// <summary>
/// The request received from the client when they want to swap 
/// an old Refresh Token for a new pair.
/// </summary>
public record RefreshTokenRequest(
    string RefreshToken
);