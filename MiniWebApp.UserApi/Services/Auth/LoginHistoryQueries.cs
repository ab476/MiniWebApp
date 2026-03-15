using MiniWebApp.UserApi.Domain;
using System.Net;

namespace MiniWebApp.UserApi.Services.Auth;

public sealed record LoginHistoryResponse
{
    public Guid Id { get; init; }
    public Guid? UserId { get; init; }
    public DateTime LoginTime { get; init; }
    public IPAddress? IpAddress { get; init; }
    public string? DeviceInfo { get; init; }
    public string? Location { get; init; }
    public bool IsSuccessful { get; init; }
}

public sealed record CreateLoginHistoryRequest(
    Guid? UserId,
    Guid? TenantId,
    IPAddress? IpAddress,
    string? DeviceInfo,
    string? Location,
    bool IsSuccessful
);


public sealed class LoginHistoryQueries(UserDbContext db) : ILoginHistoryQueries
{
    public async Task<Outcome<IReadOnlyList<LoginHistoryResponse>>> GetRecentLoginsAsync(
        Guid userId,
        int limit = 10,
        CancellationToken ct = default)
    {
        var history = await db.LoginHistories
            .TagWith("LoginHistoryQueries.GetRecentLoginsAsync: Fetching audit trail")
            .AsNoTracking()
            .Where(lh => lh.UserId == userId)
            .OrderByDescending(lh => lh.LoginTime)
            .Take(limit)
            .Select(lh => new LoginHistoryResponse
            {
                Id = lh.Id,
                UserId = lh.UserId,
                LoginTime = lh.LoginTime,
                IpAddress = lh.IpAddress,
                DeviceInfo = lh.DeviceInfo,
                Location = lh.Location,
                IsSuccessful = lh.IsSuccessful
            })
            .ToListAsync(ct);

        return (StatusCodes.Status200OK, history);
    }
}
