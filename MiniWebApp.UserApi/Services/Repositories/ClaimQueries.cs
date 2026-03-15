using MiniWebApp.UserApi.Domain;

namespace MiniWebApp.UserApi.Services.Repositories;

public sealed class ClaimQueries(
    UserDbContext db)
    : IClaimQueries
{
    public async Task<Outcome<ClaimResponse>> GetClaimAsync(
        GetClaimRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.ClaimCode))
        {
            return ("Provide Code to search.", StatusCodes.Status400BadRequest);
        }

        var normalizedCode = request.ClaimCode.ToLowerInvariant().Trim();
        var query = db.Claims.AsNoTracking().Where(x => x.ClaimCode == normalizedCode);


        var result = await query
            .TagWith("ClaimQueries.GetClaimAsync: Fetching claim by ID or Code")
            .ProjectToResponse()
            .FirstOrDefaultAsync(ct);

        return result is null
            ? ("Claim not found.", StatusCodes.Status404NotFound)
            : (StatusCodes.Status200OK, result);
    }

    public async Task<Outcome<ClaimResponse[]>> ListClaimsAsync(CancellationToken ct)
    {

        var items = await db.Claims
            .AsNoTracking()
            .TagWith("ClaimQueries.ListClaimsAsync: Fetching all claims ordered by Code")
            .OrderBy(x => x.ClaimCode)
            .ProjectToResponse()
            .ToArrayAsync(ct);

        return (StatusCodes.Status200OK, items);
    }
    public async Task<HashSet<string>> GetExistingClaimCodesAsync(CancellationToken ct)
    {
        var existingCodes = await db.Claims
            .AsNoTracking()
            .TagWith("ClaimQueries.GetExistingClaimCodesAsync: Fetching all existing claim codes for seeding process")
            .Select(c => c.ClaimCode)
            .ToHashSetAsync(StringComparer.OrdinalIgnoreCase, ct);
        return existingCodes;
    }
}
