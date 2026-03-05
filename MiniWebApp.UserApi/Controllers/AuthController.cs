//using Microsoft.AspNetCore.Identity.Data;
//using MiniWebApp.UserApi.Services.Auth;
//using LoginRequest = MiniWebApp.UserApi.Services.LoginRequest;

//namespace MiniWebApp.UserApi.Controllers;

//[ApiController]
//[Route("api/auth")]
//public class AuthController(AuthService authService) : ApiControllerBase
//{
//    [HttpPost("login")]
//    public async Task<Outcome<AuthResponse>> LoginAsync([FromBody] LoginRequest request, CancellationToken ct)
//    {
//        string ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault()
//                           ?? HttpContext.Connection.RemoteIpAddress?.ToString()
//                           ?? "Unknown";

//        string userAgent = Request.Headers.UserAgent.ToString();

//        var enrichedRequest = request with { IpAddress = ipAddress, DeviceInfo = userAgent };

//        return await authService.LoginAsync(enrichedRequest, ct);
//    }

//    [HttpPost("refresh")]
//    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
//    public async Task<Outcome<AuthResponse>> RefreshAsync([FromBody] TokenRequest request)
//    {
//        return await authService.RefreshTokenAsync(request);
//    }

//    [HttpPost("logout")]
//    [Authorize]
//    public async Task<Outcome> LogoutAsync()
//    {
//        // Get UserId from ClaimsPrincipal (User.GetUserId() is a common extension)
//        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
//        return await authService.LogoutAsync(userId);
//    }
//}


//public record TokenRequest(string AccessToken, string RefreshToken);
//public record AuthResponse(string AccessToken, string RefreshToken, DateTime Expiry, UserDto User);
