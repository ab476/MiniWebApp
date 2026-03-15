namespace MiniWebApp.UserApi.Services.Auth;

public interface ILoginHistoryRepository
{
    Task<Outcome<Guid>> LogAsync(CreateLoginHistoryRequest request, CancellationToken ct = default);
    Task<Outcome> PurgeOldLogsAsync(int daysToKeep, CancellationToken ct = default);
}