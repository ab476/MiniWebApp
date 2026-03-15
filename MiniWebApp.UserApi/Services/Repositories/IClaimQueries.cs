namespace MiniWebApp.UserApi.Services.Repositories;

public interface IClaimQueries
{
    Task<Outcome<ClaimResponse>> GetClaimAsync(
        GetClaimRequest request,
        CancellationToken ct = default
    );
    Task<HashSet<string>> GetExistingClaimCodesAsync(CancellationToken ct);
    Task<Outcome<ClaimResponse[]>> ListClaimsAsync(
        CancellationToken ct = default
    );
}