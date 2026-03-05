using System.Security.Cryptography;
using System.Text;
using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Auth;

public class RefreshTokenService(UserDbContext context) : IRefreshTokenService
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

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = hashedToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(ExpiryDays),
            CreatedByIp = ipAddress
        };

        // Save to database
        RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync(ct);

        return (StatusCodes.Status201Created, new TokenResponse(
            RefreshToken: rawRefreshToken,
            RefreshTokenExpiration: refreshToken.ExpiresAt
        ));
    }
    public async Task<Outcome<TokenResponse>> RotateTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken ct = default)
    {
        var incomingHash = ComputeHash(request.RefreshToken);

        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        try
        {
            var storedToken = await RefreshTokens // 🔹 Using the property
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.TokenHash == incomingHash, ct);

            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
            {
                return ("Invalid credentials", StatusCodes.Status401Unauthorized);
            }

            if (storedToken.RevokedAt != null)
            {
                await RevokeAllUserTokensAsync(storedToken.UserId, ct);
                await context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                return ("Invalid credentials", StatusCodes.Status401Unauthorized);
            }

            var newRawRefreshToken = GenerateSecureRandomString();
            var newHashedToken = ComputeHash(newRawRefreshToken);

            var newToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = storedToken.UserId,
                TokenHash = newHashedToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(ExpiryDays),
                CreatedByIp = ipAddress
            };

            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.ReplacedByTokenId = newToken.Id;

            RefreshTokens.Add(newToken);    // 🔹 Using the property
            RefreshTokens.Update(storedToken); // 🔹 Using the property

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return (
                StatusCodes.Status200OK, 
                new TokenResponse(
                    RefreshToken: newRawRefreshToken,
                    RefreshTokenExpiration: newToken.ExpiresAt
                )
            );
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<Outcome> RevokeAllUserTokensAsync(Guid userId, CancellationToken ct = default)
    {
        int affectedRows = await RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.RevokedAt, DateTime.UtcNow), ct);

        return affectedRows > 0
            ? StatusCodes.Status200OK
            : ("No active sessions found.", StatusCodes.Status404NotFound);
    }

    private static byte[] ComputeHash(string input)
    {
        // Safe conversion to bytes regardless of string content
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        return SHA256.HashData(inputBytes);
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