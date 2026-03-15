using MiniWebApp.UserApi.Services.Auth;
using System.Linq.Expressions;

namespace MiniWebApp.UserApi.Models;


/// <summary>
/// Provides mapping extensions for <see cref="RefreshToken"/> entities to <see cref="RefreshTokenResponse"/> DTOs.
/// </summary>
public static class RefreshTokenMappingsExtensions
{
    /// <summary>
    /// Defines the expression for mapping a <see cref="RefreshToken"/> to a <see cref="RefreshTokenResponse"/>.
    /// </summary>
    private static readonly Expression<Func<RefreshToken, RefreshTokenResponse>> MappingExpression =
        token => new RefreshTokenResponse
        {
            Id = token.Id,
            UserId = token.UserId,
            CreatedAt = token.CreatedAt,
            ExpiresAt = token.ExpiresAt,
            RevokedAt = token.RevokedAt,
            ReplacedByTokenId = token.ReplacedByTokenId,
            // IsExpired, IsRevoked, IsActive are computed properties in RefreshTokenResponse.
            // The actual RefreshToken string is not stored in the entity and must be set separately if available.
        };

    /// <summary>
    /// A compiled function for mapping a single <see cref="RefreshToken"/> to a <see cref="RefreshTokenResponse"/>.
    /// </summary>
    private static readonly Func<RefreshToken, RefreshTokenResponse> CompiledMapper =
        MappingExpression.Compile();

    /// <summary>
    /// Projects an <see cref="IQueryable{RefreshToken}"/> to an <see cref="IQueryable{RefreshTokenResponse}"/> (EF Core optimized).
    /// </summary>
    /// <param name="query">The original queryable of RefreshToken entities.</param>
    /// <returns>A queryable of RefreshTokenResponse DTOs.</returns>
    public static IQueryable<RefreshTokenResponse> ProjectToResponse(this IQueryable<RefreshToken> query)
    {
        return query.Select(MappingExpression);
    }

    /// <summary>
    /// Converts a single <see cref="RefreshToken"/> entity to a <see cref="RefreshTokenResponse"/>.
    /// </summary>
    /// <param name="refreshToken">The RefreshToken entity to convert.</param>
    /// <returns>A RefreshTokenResponse DTO.</returns>
    public static RefreshTokenResponse ToResponse(this RefreshToken refreshToken)
    {
        return CompiledMapper.Invoke(refreshToken);
    }
}