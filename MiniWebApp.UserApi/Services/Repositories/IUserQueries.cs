namespace MiniWebApp.UserApi.Services.Repositories;

public interface IUserQueries
{
    Task<Outcome<UserResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<HashSet<string>> GetExistingUserEmailsAsync(CancellationToken ct = default);
    Task<Outcome<IReadOnlyList<UserResponse>>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default);
}