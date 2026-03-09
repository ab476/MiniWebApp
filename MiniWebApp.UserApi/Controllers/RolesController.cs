using MiniWebApp.Core.Security;
using MiniWebApp.UserApi.Models.Roles;
using MiniWebApp.UserApi.Services.Repositories;

namespace MiniWebApp.UserApi.Controllers;

[Route("api/roles")]
public class RolesController(IRoleQueries roleService, IRoleRepository roleRepository, IUserContext _user) : ApiControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize(Policy = AppPermissions.Roles.Read)]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<Outcome<RoleResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await roleService.GetByIdAsync(id, ct);
    }

    [HttpGet]
    [Authorize(Policy = AppPermissions.Roles.Read)]
    [ProducesResponseType(typeof(IReadOnlyList<RoleResponse>), StatusCodes.Status200OK)]
    public async Task<Outcome<IReadOnlyList<RoleResponse>>> GetPagedAsync(
        [FromQuery] Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        return await roleService.GetPagedAsync(tenantId, page, pageSize, ct);
    }

    [HttpPost]
    [Authorize(Policy = AppPermissions.Roles.Write)]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<Outcome<RoleResponse>> CreateAsync(
        [FromBody] CreateRoleRequest request,
        CancellationToken ct = default)
    {
        if (request.TenantId == Guid.Empty)
        {
            request = request with { TenantId = _user.TenantId };
        }
       
        await ValidateAsync(request, ct);
        var outcome = await roleRepository.CreateAsync(request, ct);
        if (outcome.IsSuccess)
        {
            var id = outcome.Value!.Id;
            Response.Headers.Location = Url.Action(
                nameof(GetByIdAsync),
                new { id }
            );
        }
        return outcome;
    }

    [HttpPut("{roleId:guid}")]
    [Authorize(Policy = AppPermissions.Roles.Write)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<Outcome> UpdateAsync(
        Guid roleId,
        [FromBody] UpdateRoleRequest request,
        CancellationToken ct = default)
    {
        await ValidateAsync(request, ct);
        return await roleRepository.UpdateAsync(roleId, request, ct);
    }

    [HttpDelete("{roleId:guid}")]
    [Authorize(Policy = AppPermissions.Roles.Manage)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<Outcome> DeleteAsync(Guid roleId, CancellationToken ct = default)
    {
        return await roleRepository.DeleteAsync(roleId, ct);
    }
}
