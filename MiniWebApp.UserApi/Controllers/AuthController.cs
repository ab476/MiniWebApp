using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniWebApp.Core.Security;
using MiniWebApp.UserApi.Services.Auth;
using System.Net;

namespace MiniWebApp.UserApi.Controllers;

[Route("api/auth")]
public class AuthController(IAuthService authService, IUserContext userContext) : ApiControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<Outcome<LoginResponse>> LoginAsync(
        [FromBody] LoginRequest request,
        CancellationToken ct = default)
    {
        // Extracting metadata for audit logging in AuthService
        var ipAddress = HttpContext.Connection.RemoteIpAddress;
        var deviceInfo = Request.Headers.UserAgent.ToString();

        return await authService.LoginAsync(request, ipAddress, deviceInfo, ct);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Outcome<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<Outcome<LoginResponse>> RefreshTokenAsync(
        [FromBody] RefreshTokenRequest request,
        CancellationToken ct = default)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress;

        return await authService.RefreshTokenAsync(request.RefreshToken, ipAddress, ct);
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<Outcome> LogoutAsync(CancellationToken ct = default)
    {
        var userId = userContext.UserId;

        return await authService.LogoutAsync(userId, ct);
    }
}

// Supporting record for the refresh request body
public record RefreshTokenRequest(string RefreshToken);