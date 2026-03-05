using MiniWebApp.UserApi.Models.Permissions;
using MiniWebApp.UserApi.Services.Permissions;

namespace MiniWebApp.UserApi.Controllers;


[Route("api/permissions")]
public class PermissionsController(IPermissionQueries queries) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = AppPermissions.Permissions.Read)]
    public async Task<Outcome<PermissionResponse[]>> ListPermissions(CancellationToken ct)
        => await queries.ListPermissions(ct);

    [HttpGet("{idOrCode}")]
    [Authorize(Policy = AppPermissions.Permissions.Read)]
    public async Task<Outcome<PermissionResponse>> GetPermission(string idOrCode, CancellationToken ct)
    {
        GetPermissionRequest request = Guid.TryParse(idOrCode, out Guid guidId)
            ? new GetPermissionRequest() { Id = guidId }
            : new GetPermissionRequest() { Code = idOrCode };
        
        await ValidateAsync(request, ct);

        return await queries.GetPermissionAsync(request, ct);
    }
}
