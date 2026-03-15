namespace MiniWebApp.UserApi.Services.Auth;

public interface ILoginHistoryQueries
{
    Task<Outcome<IReadOnlyList<LoginHistoryResponse>>> GetRecentLoginsAsync(Guid userId, int limit = 10, CancellationToken ct = default);
}