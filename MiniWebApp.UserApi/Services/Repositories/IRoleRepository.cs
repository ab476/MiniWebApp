namespace MiniWebApp.UserApi.Services.Repositories;

/// <summary>
/// Defines the contract for managing roles within a multi-tenant environment.
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Creates a new role for a specific tenant.
    /// </summary>
    /// <param name="request">The role details, including an optional target TenantId.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// An <see cref="Outcome{T}"/> containing the created <see cref="RoleResponse"/> on success, 
    /// or a forbidden error if a non-SuperAdmin attempts to create a role for another tenant.
    /// </returns>
    Task<Outcome<RoleResponse>> CreateAsync(CreateRoleRequest request, CancellationToken ct = default);

    /// <summary>
    /// Creates multiple roles in a single batch for a specific tenant.
    /// </summary>
    /// <param name="requests">The collection of role creation requests.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// An <see cref="Outcome{T}"/> containing an array of <see cref="RoleResponse"/> on success. 
    /// Returns a bad request if the batch spans multiple tenants, or forbidden if tenant access is denied.
    /// </returns>
    Task<Outcome<RoleResponse[]>> CreateBulkAsync(IEnumerable<CreateRoleRequest> requests, CancellationToken ct = default);

    /// <summary>
    /// Deletes an existing role based on the role code and tenant context.
    /// </summary>
    /// <param name="request">The deletion request containing the RoleCode and target TenantId.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A successful <see cref="Outcome"/> if the role was removed; 
    /// otherwise, a 404 Not Found or a 403 Forbidden outcome.
    /// </returns>
    Task<Outcome> DeleteAsync(DeleteRoleRequest request, CancellationToken ct = default);

    /// <summary>
    /// Updates the properties of an existing role.
    /// </summary>
    /// <param name="request">The update request containing the role code and new display name.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A successful <see cref="Outcome"/> if the update was applied; 
    /// otherwise, a 404 Not Found or a 403 Forbidden outcome.
    /// </returns>
    Task<Outcome> UpdateAsync(UpdateRoleRequest request, CancellationToken ct = default);
}