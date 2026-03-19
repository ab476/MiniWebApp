using MiniWebApp.Core.Security;
using MiniWebApp.UserApi.Services.Repositories;
using System.Net;

namespace MiniWebApp.UserApi.Services.Auth;

public sealed class AuthService(
    IUserRepository userRepository,
    IUserQueries userQueries,
    IRefreshTokenService refreshTokenService,
    IUserRoleQueries userRoleQueries,
    IJwtTokenGenerator jwtTokenGenerator,
    IRoleClaimQueries roleClaimQueries,
    ILoginHistoryRepository loginHistoryRepository,
    ILogger<AuthService> logger) : IAuthService
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
            user.Permissions); // Add permissions if applicable

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

        var newToken = rotationResult.Value!;

        // 2. Fetch User to rebuild JWT claims
        var userResponse = await userQueries.GetByIdAsync(newToken.UserId, ct);
        if (!userResponse.IsSuccess)
        {
            logger.LogWarning("Failed to retrieve user {UserId} for refresh token rotation: {Reason}", newToken.UserId, userResponse.Error);
            return userResponse.ToFailure<LoginResponse>();
        }
        var user = userResponse.Value!;

        // Fetch user roles
        var rolesOutcome = await userRoleQueries.GetRolesByUserAsync(user.Id, ct);
        if (!rolesOutcome.IsSuccess)
        {
            logger.LogWarning("Failed to retrieve roles for user {UserId} during refresh token rotation: {Reason}", user.Id, rolesOutcome.Error);
            return rolesOutcome.ToFailure<LoginResponse>();
        }
        var userRoles = rolesOutcome.Value.Select(r => r.RoleCode).ToArray();

        // Fetch permissions based on roles
        var permissionsOutcome = await roleClaimQueries.GetClaimsByRolesAsync(userRoles, ct);
        if (!permissionsOutcome.IsSuccess)
        {
            logger.LogWarning("Failed to retrieve permissions for user {UserId} during refresh token rotation: {Reason}", user.Id, permissionsOutcome.Error);
            return permissionsOutcome.ToFailure<LoginResponse>();
        }
        var userPermissions = permissionsOutcome.Value.Select(p => p.ClaimCode).ToArray();

        // 3. Generate new JWT
        var jwtUser = new JwtUser(
            user.Id,
            user.Email,
            user.UserName,
            user.TenantId,
            userRoles,
            userPermissions);
        var newAccessToken = jwtTokenGenerator.Generate(jwtUser);

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

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("User {UserId} logged out. All sessions revoked.", userId);
        }
        return StatusCodes.Status200OK;
    }
}

public record LoginResponse(string AccessToken, string RefreshToken, DateTime RefreshTokenExpires);
public record LoginRequest(string Identifier, string Password);