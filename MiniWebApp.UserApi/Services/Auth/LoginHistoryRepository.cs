using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Auth;

public sealed class LoginHistoryRepository(UserDbContext db) : ILoginHistoryRepository
{
    public async Task<Outcome<Guid>> LogAsync(
        CreateLoginHistoryRequest request,
        CancellationToken ct = default)
    {
        var entry = new LoginHistory
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            TenantId = request.TenantId,
            LoginTime = DateTime.UtcNow,
            IpAddress = request.IpAddress,
            DeviceInfo = request.DeviceInfo,
            Location = request.Location,
            IsSuccessful = request.IsSuccessful
        };

        await db.LoginHistories.AddAsync(entry, ct);
        await db.SaveChangesAsync(ct);

        return (StatusCodes.Status201Created, entry.Id);
    }

    /// <summary>
    /// Optional: Cleanup old logs to prevent table bloat
    /// </summary>
    public async Task<Outcome> PurgeOldLogsAsync(int daysToKeep, CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.AddDays(-daysToKeep);

        await db.LoginHistories
            .TagWith("LoginHistoryRepository.PurgeOldLogsAsync: Maintenance - deleting logs older than threshold")
            .Where(lh => lh.LoginTime < cutoff)
            .ExecuteDeleteAsync(ct);

        return StatusCodes.Status200OK;
    }
}