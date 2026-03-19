using MiniWebApp.Core.Security;
using MiniWebApp.UserApi.Services.Repositories;

namespace MiniWebApp.UserApi.Controllers;

[Route("api/roles")]
public class RolesController(IRoleQueries roleService, IRoleRepository roleRepository, ITenantProvider _tenant) : ApiControllerBase
{
    [HttpGet("{roleCode}")]
    [Authorize(Policy = AppPermissions.Roles.Read)]
    public async Task<Outcome<RoleResponse>> GetByRoleCodeAsync(string roleCode, CancellationToken ct = default)
    {
        return await roleService.GetByRoleCodeAsync(roleCode, ct);
    }

    [HttpGet]
    [Authorize(Policy = AppPermissions.Roles.Read)]
    public async Task<Outcome<List<RoleResponse>>> GetPagedAsync(
        [FromQuery] Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        return await roleService.GetPagedAsync(tenantId, page, pageSize, ct);
    }

    [HttpPost]
    [Authorize(Policy = AppPermissions.Roles.Write)]
    public async Task<Outcome<RoleResponse>> CreateAsync(
        [FromBody] CreateRoleRequest request,
        CancellationToken ct = default)
    {
        if (request.TenantId == Guid.Empty)
        {
            request = request with { TenantId = _tenant.TenantId };
        }
       
        await ValidateAsync(request, ct);
        var outcome = await roleRepository.CreateAsync(request, ct);
        if (outcome.IsSuccess)
        {
            var roleCode = outcome.Value!.RoleCode;
            Response.Headers.Location = Url.Action(
                nameof(GetByRoleCodeAsync),
                new { roleCode }
            );
        }
        return outcome;
    }

    [HttpPut("{roleCode}")]
    [Authorize(Policy = AppPermissions.Roles.Write)]
    public async Task<Outcome> UpdateAsync(
        [FromRoute]string roleCode,
        [FromBody] UpdateRoleRequest request,
        CancellationToken ct = default)
    {
        request = request with { RoleCode = roleCode };
        await ValidateAsync(request, ct);
        return await roleRepository.UpdateAsync(request, ct);
    }

    [HttpDelete("{roleCode}")]
    [Authorize(Policy = AppPermissions.Roles.Manage)]
    public async Task<Outcome> DeleteAsync(
        [FromRoute] string roleCode,
        [FromBody] DeleteRoleRequest request,
        CancellationToken ct = default)
    {
        request = request with { RoleCode = roleCode };
        await ValidateAsync(request, ct);
        return await roleRepository.DeleteAsync(request, ct);
    }
}
