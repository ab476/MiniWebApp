using MiniWebApp.UserApi.Services.Repositories;

namespace MiniWebApp.UserApi.Controllers;

[Route("api/claims")]
public class ClaimsController(IClaimQueries queries) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = AppPermissions.Permissions.Read)]
    public async Task<Outcome<ClaimResponse[]>> ListClaims(CancellationToken ct)
        => await queries.ListClaimsAsync(ct);

    [HttpGet("{claimCode}")]
    [Authorize(Policy = AppPermissions.Permissions.Read)]
    public async Task<Outcome<ClaimResponse>> GetClaim([FromRoute] GetClaimRequest request, CancellationToken ct)
    {
        await ValidateAsync(request, ct);

        return await queries.GetClaimAsync(request, ct);
    }
}