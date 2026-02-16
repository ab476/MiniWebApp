using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniWebApp.Core.Auth;
using MiniWebApp.Core.Common;
using MiniWebApp.UserApi.Application.Tenants;
using MiniWebApp.UserApi.Contracts.Tenants;

namespace MiniWebApp.UserApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController(TenantService tenantService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize(Policy = AppPermissions.Tenants.Read)]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<Outcome<TenantResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await tenantService.GetByIdAsync(id, ct);
    }

    [HttpGet]
    [Authorize(Policy = AppPermissions.Tenants.Read)]
    [ProducesResponseType(typeof(IReadOnlyList<TenantResponse>), StatusCodes.Status200OK)]
    public async Task<Outcome<IReadOnlyList<TenantResponse>>> GetPagedAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        return await tenantService.GetPagedAsync(page, pageSize, ct);
    }

    [HttpPost]
    [Authorize(Policy = AppPermissions.Tenants.Write)]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateTenantRequest request,
        CancellationToken ct = default)
    {
        var outcome = await tenantService.CreateAsync(request, ct);
        if (!outcome.IsSuccess)
        {
            return outcome.Convert();
        }

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = outcome.Value!.Id },
            outcome.Value);
    }

    [HttpPut("{tenantId:guid}")]
    [Authorize(Policy = AppPermissions.Tenants.Write)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<Outcome> UpdateAsync(
        Guid tenantId,
        [FromBody] UpdateTenantRequest request,
        CancellationToken ct = default)
    {
        var outcome = await tenantService.UpdateAsync(tenantId, request, ct);
        return outcome;
    }

    [HttpPost("{tenantId:guid}/activate")]
    [Authorize(Policy = AppPermissions.Tenants.Manage)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<Outcome> ActivateAsync(Guid tenantId, CancellationToken ct = default)
    {
        var outcome = await tenantService.ActivateAsync(new ActivateTenantRequest(tenantId), ct);
        return outcome;
    }

    [HttpPost("{tenantId:guid}/deactivate")]
    [Authorize(Policy = AppPermissions.Tenants.Manage)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<Outcome> DeactivateAsync(Guid tenantId, CancellationToken ct = default)
    {
        var outcome = await tenantService.DeactivateAsync(new DeactivateTenantRequest(tenantId), ct);
        return outcome;
    }

    [HttpDelete("{tenantId:guid}")]
    [Authorize(Policy = AppPermissions.Tenants.Manage)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<Outcome> DeleteAsync(Guid tenantId, CancellationToken ct = default)
    {
        var outcome = await tenantService.DeleteAsync(tenantId, ct);
        return outcome;
    }
}
