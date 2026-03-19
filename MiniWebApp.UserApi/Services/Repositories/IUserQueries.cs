namespace MiniWebApp.UserApi.Services.Repositories;

public interface IUserQueries
{
    Task<Outcome<UserResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Outcome<HashSet<string>>> GetExistingUserEmailsAsync(CancellationToken ct = default);
    Task<Outcome<List<UserResponse>>> GetByEmailsAsync(HashSet<string> emails, CancellationToken ct = default);
    Task<Outcome<List<UserResponse>>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default);
}