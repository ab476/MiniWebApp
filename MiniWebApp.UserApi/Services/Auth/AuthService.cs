//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using MiniWebApp.Core.Security;
//using MiniWebApp.UserApi.Domain;
//using MiniWebApp.UserApi.Domain.Models;
//using MiniWebApp.UserApi.Services.RefreshToken;

//namespace MiniWebApp.UserApi.Services.Auth;

//public class AuthService(
//    UserDbContext db,
//    IJwtTokenGenerator tokenService,
//    IRefreshTokenService refreshTokenService,
//    IPasswordHasher<TUser> passwordHasher)
//{
//    public async Task<Outcome<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
//    {
//        var user = await db.TUsers
//            .TagWith($"{nameof(AuthService)}.{nameof(LoginAsync)}")
//            .Include(u => u.UserRoles)
//            .FirstOrDefaultAsync(u =>
//                u.NormalizedEmail == request.Email.ToUpperInvariant() &&
//                u.TenantId == request.TenantId, ct);

//        var verificationResult = user is not null
//            ? passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
//            : PasswordVerificationResult.Failed;

//        bool isValid = verificationResult != PasswordVerificationResult.Failed;

//        // Log history independently of success
//        db.TLoginHistories.Add(new TLoginHistory
//        {
//            UserId = user?.Id ?? Guid.Empty,
//            TenantId = request.TenantId,
//            LoginTime = DateTime.UtcNow,
//            IsSuccessful = isValid,
//            IpAddress = request.IpAddress,
//            DeviceInfo = request.DeviceInfo
//        });

//        if (!isValid || user == null)
//        {
//            await db.SaveChangesAsync(ct);
//            return ("Invalid credentials or tenant mismatch.", StatusCodes.Status401Unauthorized);
//        }

//        if (user.Status != UserStatus.Active)
//            return ($"Account is {user.Status}.", StatusCodes.Status403Forbidden);

//        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
//        {
//            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
//        }

//        // 🔹 Leverage RefreshTokenService for initial creation
//        var tokenPair = await refreshTokenService.CreateInitialTokensAsync(user.Id, request.IpAddress, ct);

//        // Update user metadata
//        user.LastLoginAt = DateTime.UtcNow;
//        await db.SaveChangesAsync(ct);

//        return (StatusCodes.Status200OK, MapToAuthResponse(user, tokenPair));
//    }

//    public async Task<Outcome<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken ct = default)
//    {
//        // 🔹 Delegate rotation logic to the specialized service
//        // This handles hashing, expiry checks, and replay attack detection internally
//        var tokenPair = await refreshTokenService.RotateTokenAsync(request, ipAddress, ct);

//        if (tokenPair == null)
//            return ("Session expired or invalid. Please log in again.", StatusCodes.Status401Unauthorized);

//        // Fetch user to populate the AuthResponse (roles, profile info, etc.)
//        // Note: RotateTokenAsync likely already ensures the user exists/is active
//        var user = await db.TUsers
//            .Include(u => u.UserRoles)
//            .FirstOrDefaultAsync(u => u.Id == GetUserIdFromToken(tokenPair.AccessToken), ct);

//        if (user == null || user.Status != UserStatus.Active)
//            return ("User account is no longer active.", StatusCodes.Status403Forbidden);

//        return (StatusCodes.Status200OK, MapToAuthResponse(user, tokenPair));
//    }

//    public async Task<Outcome> LogoutAsync(Guid userId, CancellationToken ct = default)
//    {
//        // 🔹 Global logout: Revoke all tokens for this user
//        await refreshTokenService.RevokeAllUserTokensAsync(userId, ct);
//        return StatusCodes.Status200OK;
//    }

//    private AuthResponse MapToAuthResponse(TUser user, TokenResponse tokenPair)
//    {
//        return new AuthResponse(
//            tokenPair.AccessToken,
//            tokenPair.RefreshToken,
//            tokenPair.RefreshTokenExpiration,
//            user.ToDto());
//    }

//    private Guid GetUserIdFromToken(string token)
//    {
//        // Helper to extract Subject claim from JWT if needed, 
//        // or you could modify IRefreshTokenService to return the User object
//        return tokenService.GetUserIdFromExpiredToken(token);
//    }
//}