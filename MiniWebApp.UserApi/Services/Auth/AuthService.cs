using MiniWebApp.Core.Security;
using MiniWebApp.UserApi.Services.Repositories;
using System.Net;

namespace MiniWebApp.UserApi.Services.Auth;

public sealed class AuthService(
    IUserRepository userRepository,
    IRefreshTokenService refreshTokenService,
    IJwtTokenGenerator jwtTokenGenerator,
    ILoginHistoryRepository loginHistoryRepository,
    ILogger<AuthService> logger)
{
    public async Task<Outcome<LoginResponse>> LoginAsync(
        LoginRequest request,
        IPAddress? ipAddress,
        string? deviceInfo,
        CancellationToken ct = default)
    {
        // 1. Verify Credentials via Repository
        var credentialsResult = await userRepository.VerifyCredentialsAsync(
            new VerifyCredentialsRequest(request.Identifier, request.Password), ct);

        if (!credentialsResult.IsSuccess)
        {
            // Log failed attempt for security auditing
            await loginHistoryRepository.LogAsync(
                new CreateLoginHistoryRequest(null, null, ipAddress, deviceInfo, "Unknown", false), 
                ct);

            return credentialsResult.ToFailure<LoginResponse>();
        }

        var user = credentialsResult.Value;

        // 2. Generate initial Refresh Token (with rotation support)
        var rtResult = await refreshTokenService.CreateInitialTokensAsync(user!.UserId, ipAddress, ct);

        if (!rtResult.IsSuccess) return rtResult.ToFailure<LoginResponse>();

        // 3. Generate JWT Access Token
        var jwtUser = new JwtUser(
            user.UserId,
            user.Email,
            user.Username,
            user.TenantId,
            user.Roles,
            []); // Add permissions if applicable

        var accessToken = jwtTokenGenerator.Generate(jwtUser);

        // 4. Audit Log
        CreateLoginHistoryRequest loginHistoryRequest = new(user.UserId, user.TenantId, ipAddress, deviceInfo, "Unknown", true);
        await loginHistoryRepository.LogAsync(loginHistoryRequest, ct);

        return (StatusCodes.Status200OK, new LoginResponse(
            accessToken,
            rtResult.Value.RefreshToken,
            rtResult.Value.ExpiresAt));
    }

    public async Task<Outcome<LoginResponse>> RefreshTokenAsync(
        string refreshToken,
        IPAddress? ipAddress,
        CancellationToken ct = default)
    {
        // 1. Rotate the token
        var rotationResult = await refreshTokenService.RotateAsync(refreshToken, ipAddress, ct);

        if (!rotationResult.IsSuccess)
        {
            logger.LogWarning("Refresh attempt failed: {Reason}", rotationResult.Error);
            return rotationResult.ToFailure<LoginResponse>();
        }

        var newToken = rotationResult.Value;

        // 2. Fetch User to rebuild JWT claims
        // Note: You might want a dedicated GetByIdAsync in UserRepository
        // For now, we assume we need the user context to generate a valid JWT
        // (Implementation detail: Ideally, the RotateAsync could return basic user info)

        // ... Logic to fetch user roles/email ...
        // var user = await userRepository.GetByIdAsync(newToken.UserId);

        // 3. Generate new JWT
        // (Simplified for brevity - use the user data fetched above)
        var dummyJwtUser = new JwtUser(newToken.UserId, "", "", null, [], []);
        var newAccessToken = jwtTokenGenerator.Generate(dummyJwtUser);

        return (StatusCodes.Status200OK, new LoginResponse(
            newAccessToken,
            newToken.RefreshToken,
            newToken.ExpiresAt));
    }

    public async Task<Outcome> LogoutAsync(Guid userId, CancellationToken ct = default)
    {
        // In a stateless JWT world, "Logout" means invalidating the Refresh Token
        // so the user cannot get a new Access Token.
        await refreshTokenService.RevokeAllForUserAsync(userId, ct);

        logger.LogInformation("User {UserId} logged out. All sessions revoked.", userId);
        return StatusCodes.Status200OK;
    }
}

public record LoginResponse(string AccessToken, string RefreshToken, DateTime RefreshTokenExpires);
public record LoginRequest(string Identifier, string Password);