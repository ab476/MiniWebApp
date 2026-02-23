using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniWebApp.Core.Auth;
using MiniWebApp.Core.Common;
using MiniWebApp.Core.Controllers;
using MiniWebApp.Core.Models;
using MiniWebApp.UserApi.Models.Permissions;
using MiniWebApp.UserApi.Services.Permissions;
using System.Reflection.Metadata;

namespace MiniWebApp.UserApi.Controllers;


[Route("api/permissions")]
public class PermissionsController(IPermissionQueries queries) : ApiControllerBase
{
    [HttpGet]
    //[Authorize(Policy = AppPermissions.Permissions.Read)]
    public async Task<Outcome<PagedResponse<PermissionResponse>>> GetPaged([FromQuery] PagedRequest request, CancellationToken ct)
        => await queries.GetPagedAsync(request, ct);

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
