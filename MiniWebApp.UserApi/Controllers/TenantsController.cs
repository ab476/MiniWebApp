using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniWebApp.Core.Auth;
using MiniWebApp.Core.Common;
using MiniWebApp.Core.Controllers;
using MiniWebApp.UserApi.Models.Tenants;
using MiniWebApp.UserApi.Services.Tenants;

namespace MiniWebApp.UserApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController(TenantService tenantService) : ApiControllerBase
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
    public async Task<Outcome<TenantResponse>> CreateAsync(
        [FromBody] CreateTenantRequest request,
        CancellationToken ct = default)
    {
        await ValidateAsync(request, ct);
        var outcome = await tenantService.CreateAsync(request, ct);
        if (outcome.IsSuccess)
        {
            var tenant = outcome.Value!;

            var location = Url.Action(
                nameof(GetByIdAsync),
                values: new { id = tenant.Id }
            );

            Response.Headers.Location = location!;
        }


        return outcome;
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
        await ValidateAsync(request, ct);
        return await tenantService.UpdateAsync(tenantId, request, ct);
    }

    [HttpPost("{tenantId:guid}/activate")]
    [Authorize(Policy = AppPermissions.Tenants.Manage)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<Outcome> ActivateAsync([FromBody] ActivateTenantRequest request, CancellationToken ct = default)
    {
        await ValidateAsync(request, ct);
        return await tenantService.ActivateAsync(request, ct);
    }

    [HttpPost("{tenantId:guid}/deactivate")]
    [Authorize(Policy = AppPermissions.Tenants.Manage)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<Outcome> DeactivateAsync([FromBody] DeactivateTenantRequest request, CancellationToken ct = default)
    {
        await ValidateAsync(request, ct);
        return await tenantService.DeactivateAsync(request, ct);
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
