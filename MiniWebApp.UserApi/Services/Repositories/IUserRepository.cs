namespace MiniWebApp.UserApi.Services.Repositories;

public interface IUserRepository
{
    Task<Outcome<Guid[]>> CreateBulkAsync(IEnumerable<CreateUserRequest> requests, CancellationToken ct = default);
    Task<Outcome<Guid>> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<Outcome> SoftDeleteAsync(Guid userId, CancellationToken ct = default);
    Task<Outcome> UpdateStatusAsync(UpdateUserStatusRequest request, CancellationToken ct = default);
    Task<Outcome<AuthenticatedUserContext>> VerifyCredentialsAsync(VerifyCredentialsRequest request, CancellationToken ct = default);
}