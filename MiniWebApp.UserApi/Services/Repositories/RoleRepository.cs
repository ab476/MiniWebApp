using MiniWebApp.UserApi.Contracts.Roles;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Models.Roles;

namespace MiniWebApp.UserApi.Services.Repositories;


public sealed class RoleRepository(UserDbContext db) : IRoleRepository
{
    public async Task<Outcome<RoleResponse>> CreateAsync(
        CreateRoleRequest request,
        CancellationToken ct = default)
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            Name = request.Name,
            NormalizedName = request.Name.ToUpperInvariant(),
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        await db.Roles.AddAsync(role, ct);

        await db.SaveChangesAsync(ct);

        return (StatusCodes.Status201Created, role.ToResponse());
    }

    public async Task<Outcome> UpdateAsync(
        Guid roleId,
        UpdateRoleRequest request,
        CancellationToken ct = default)
    {
        // ExecuteUpdate skips the Change Tracker and hits the DB directly
        var rows = await db.Roles
            .TagWith("RoleRepository.UpdateAsync: Bulk updating role properties")
            .Where(r => r.Id == roleId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(r => r.Name, request.Name)
                .SetProperty(r => r.NormalizedName, request.Name.ToUpperInvariant())
                .SetProperty(r => r.Description, request.Description), ct);

        return rows == 1
            ? StatusCodes.Status200OK
            : ("Role not found.", StatusCodes.Status404NotFound);
    }

    public async Task<Outcome> DeleteAsync(Guid roleId, CancellationToken ct = default)
    {
        // ExecuteDelete is a high-performance database-level delete
        var rows = await db.Roles
            .TagWith("RoleRepository.DeleteAsync: Bulk deleting role by ID")
            .Where(r => r.Id == roleId)
            .ExecuteDeleteAsync(ct);

        return rows == 1
            ? StatusCodes.Status200OK
            : ("Role not found.", StatusCodes.Status404NotFound);
    }
}