using MiniWebApp.UserApi.Services.Repositories;

namespace MiniWebApp.UserApi.Controllers;

[ApiController]
[Route("api/tenants")]
public class TenantsController(ITenantRepository tenantService, ITenantQueries tenantQueries) : ApiControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize(Policy = AppPermissions.Tenants.Read)]
    public async Task<Outcome<TenantResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await tenantQueries.GetByIdAsync(id, ct);
    }

    [HttpGet]
    [Authorize(Policy = AppPermissions.Tenants.Read)]
    public async Task<Outcome<List<TenantResponse>>> GetPagedAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        return await tenantQueries.GetPagedAsync(page, pageSize, ct);
    }

    [HttpPost]
    [Authorize(Policy = AppPermissions.Tenants.Write)]
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
    public async Task<Outcome> ActivateAsync([FromBody] ActivateTenantRequest request, CancellationToken ct = default)
    {
        await ValidateAsync(request, ct);
        return await tenantService.ActivateAsync(request, ct);
    }

    [HttpPost("{tenantId:guid}/deactivate")]
    [Authorize(Policy = AppPermissions.Tenants.Manage)]
    public async Task<Outcome> DeactivateAsync([FromBody] DeactivateTenantRequest request, CancellationToken ct = default)
    {
        await ValidateAsync(request, ct);
        return await tenantService.DeactivateAsync(request, ct);
    }

    [HttpDelete("{tenantId:guid}")]
    [Authorize(Policy = AppPermissions.Tenants.Manage)]
    public async Task<Outcome> DeleteAsync(Guid tenantId, CancellationToken ct = default)
    {
        var outcome = await tenantService.DeleteAsync(tenantId, ct);
        return outcome;
    }
}
