using MiniWebApp.UserApi.Domain;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace MiniWebApp.UserApi.Services.Auth;


public sealed record RefreshTokenResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public bool IsExpired => TimeProvider.System.GetUtcNow().UtcDateTime >= ExpiresAt;
    public bool IsRevoked => RevokedAt is not null || ReplacedByTokenId is not null;
    public bool IsActive => !IsRevoked && !IsExpired;
    public DateTime? RevokedAt { get; init; }
    public string RefreshToken { get; init; } = string.Empty;
    public Guid? ReplacedByTokenId { get; init; }
}
public sealed class RefreshTokenService(
    UserDbContext db,
    TimeProvider timeProvider,
    ILogger<RefreshTokenService> logger) : IRefreshTokenService
{
    private const int TokenByteLength = 64;
    private const int ExpiryDays = 7;

    public async Task<Outcome<RefreshTokenResponse>> CreateInitialTokensAsync(Guid userId, IPAddress? ipAddress, CancellationToken ct = default)
    {
        var rawRefreshToken = GenerateSecureRandomString();
        var hashedToken = ComputeHash(rawRefreshToken);
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = hashedToken,
            ExpiresAt = utcNow.AddDays(ExpiryDays),
            CreatedAt = utcNow,
            CreatedByIp = ipAddress
        };

        await db.RefreshTokens.AddAsync(token, ct);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Initial refresh token created for User: {UserId} from IP: {IpAddress}", userId, ipAddress);

        return (StatusCodes.Status201Created, token.ToResponse() with { RefreshToken = rawRefreshToken });
    }

    public async Task<Outcome<RefreshTokenResponse>> RotateAsync(string refreshToken, IPAddress? ipAddress, CancellationToken ct = default)
    {
        var incomingHash = ComputeHash(refreshToken);
        var existingToken = await db.RefreshTokens
            .TagWith("RefreshTokenService.RotateAsync: Fetching existing token")
            .FirstOrDefaultAsync(t => t.TokenHash == incomingHash, ct);

        if (existingToken is null)
            return ("Invalid refresh token.", StatusCodes.Status401Unauthorized);

        var utcNow = timeProvider.GetUtcNow().UtcDateTime;
        var isExpired = utcNow >= existingToken.ExpiresAt;
        var isRevoked = existingToken.RevokedAt is not null || existingToken.ReplacedByTokenId is not null;
        var isActive = !isRevoked && !isExpired;

        // --- REUSE DETECTION ---
        if (!isActive)
        {
            if (existingToken.ReplacedByTokenId is not null)
            {
                logger.LogWarning("Token reuse detected for User: {UserId}", existingToken.UserId);
                await RevokeAllForUserAsync(existingToken.UserId, ct);
                return ("Refresh token reuse detected. All sessions revoked for security.", StatusCodes.Status401Unauthorized);
            }
            return ("Token is no longer valid.", StatusCodes.Status401Unauthorized);
        }

        // --- ROTATION ---
        var rawNewToken = GenerateSecureRandomString();
        var newHash = ComputeHash(rawNewToken);
        var newTokenId = Guid.NewGuid();

        // 1. Mark old token as replaced
        existingToken.RevokedAt = utcNow;
        existingToken.ReplacedByTokenId = newTokenId;

        // 2. Create the new token in the chain
        var newToken = new RefreshToken
        {
            Id = newTokenId,
            UserId = existingToken.UserId,
            TokenHash = newHash,
            ExpiresAt = utcNow.AddDays(ExpiryDays),
            CreatedAt = utcNow,
            CreatedByIp = ipAddress
        };

        await db.RefreshTokens.AddAsync(newToken, ct);

        // EF Core saves both the update to existingToken and the insert of newToken transactionally
        await db.SaveChangesAsync(ct);

        return (StatusCodes.Status200OK, newToken.ToResponse() with { RefreshToken = rawNewToken });
    }

    public async Task<Outcome<IReadOnlyList<RefreshTokenResponse>>> GetActiveTokensByUserAsync(Guid userId, CancellationToken ct = default)
    {
        var tokens = await db.RefreshTokens
            .TagWith("RefreshTokenService.GetActiveTokensByUserAsync: Listing valid sessions")
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .Select(t => new RefreshTokenResponse
            {
                Id = t.Id,
                UserId = t.UserId,
                ExpiresAt = t.ExpiresAt,
                RevokedAt = t.RevokedAt
            })
            .ToListAsync(ct);

        return (StatusCodes.Status200OK, tokens);
    }

    public async Task<Outcome> RevokeAllForUserAsync(Guid userId, CancellationToken ct = default)
    {
        await db.RefreshTokens
            .TagWith("RefreshTokenService.RevokeAllForUserAsync: Invalidating all sessions")
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ExecuteUpdateAsync(setter =>
                setter.SetProperty(t => t.RevokedAt, timeProvider.GetUtcNow().UtcDateTime),
                ct);

        return StatusCodes.Status200OK;
    }

    public async Task<Outcome> PurgeExpiredTokensAsync(CancellationToken ct = default)
    {
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;
        await db.RefreshTokens
            .TagWith("RefreshTokenService.PurgeExpiredTokensAsync: Maintenance cleanup")
            .Where(t => t.ExpiresAt <= utcNow || t.RevokedAt != null)
            .ExecuteDeleteAsync(ct);

        return StatusCodes.Status200OK;
    }

    private static byte[] ComputeHash(string input) => SHA256.HashData(Encoding.UTF8.GetBytes(input));
    private static string GenerateSecureRandomString() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(TokenByteLength));
}