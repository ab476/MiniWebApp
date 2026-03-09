//using MiniWebApp.UserApi.Domain;

//namespace MiniWebApp.UserApi.Services.Auth;

//public interface ILoginHistoryService
//{
//    Task RecordLoginAsync(LoginHistory loginHistory);
//    Task<IEnumerable<LoginHistory>> GetUserHistoryAsync(Guid userId, int limit = 10);
//}
//public class LoginHistoryService(UserDbContext userDbContext, IHttpContextAccessor httpContextAccessor) : ILoginHistoryService
//{
//    public async Task RecordLoginAsync(LoginHistory loginHistory)
//    {
//        var context = httpContextAccessor.HttpContext;

//        var history = new LoginHistory
//        {
//            Id = Guid.NewGuid(),
//            UserId = userId,
//            TenantId = tenantId,
//            LoginTime = DateTime.UtcNow,
//            IsSuccessful = isSuccessful,
//            // Grab IP Address from Connection
//            IpAddress = context?.Connection.RemoteIpAddress,
//            // Grab User Agent for DeviceInfo
//            DeviceInfo = context?.Request.Headers["User-Agent"].ToString(),
//            // Location usually requires an external GeoIP service call
//            Location = "Unknown"
//        };

//        await userDbContext.LoginHistories.AddAsync(history);
//        await userDbContext.SaveChangesAsync();
//    }

//    public async Task<IEnumerable<LoginHistory>> GetUserHistoryAsync(Guid userId, int limit = 10)
//    {
//        return await userDbContext.LoginHistories.Where(h => h.UserId == userId).ToArrayAsync();
//    }
//}