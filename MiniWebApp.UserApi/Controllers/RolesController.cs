using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniWebApp.Core.Auth;
using MiniWebApp.Core.Common;
using MiniWebApp.Core.Controllers;
using MiniWebApp.UserApi.Models.Roles;
using MiniWebApp.UserApi.Services;

namespace MiniWebApp.UserApi.Controllers;

[Route("api/roles")]
public class RolesController(RoleService roleService) : ApiControllerBase
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
        await ValidateAsync(request, ct);
        var outcome = await roleService.CreateAsync(request, ct);
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
        return await roleService.UpdateAsync(roleId, request, ct);
    }

    [HttpDelete("{roleId:guid}")]
    [Authorize(Policy = AppPermissions.Roles.Manage)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<Outcome> DeleteAsync(Guid roleId, CancellationToken ct = default)
    {
        return await roleService.DeleteAsync(roleId, ct);
    }
}
