using Microsoft.EntityFrameworkCore;
using MiniWebApp.Core.Common;
using MiniWebApp.UserApi.Contracts.Roles;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Domain.Models;

namespace MiniWebApp.UserApi.Application;

public class RoleService(UserDbContext _db)
{
    public async Task<Outcome<RoleResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var role = await _db.Roles
            .TagWith($"{nameof(RoleService)}.{nameof(GetByIdAsync)}")
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        return role is null
            ? ("Role not found.", StatusCodes.Status404NotFound)
            : (StatusCodes.Status200OK, role.ToResponse());
    }

    public async Task<Outcome<IReadOnlyList<RoleResponse>>> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var roles = await _db.Roles
            .TagWith($"{nameof(RoleService)}.{nameof(GetPagedAsync)}")
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToResponse()
            .ToListAsync(ct);

        return (StatusCodes.Status200OK, roles);
    }

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

        _db.Roles.Add(role);
        await _db.SaveChangesAsync(ct);

        return (StatusCodes.Status201Created, role.ToResponse());
    }

    public async Task<Outcome> UpdateAsync(
        Guid roleId,
        UpdateRoleRequest request,
        CancellationToken ct = default)
    {
        var rows = await _db.Roles
            .TagWith($"{nameof(RoleService)}.{nameof(UpdateAsync)}")
            .Where(r => r.Id == roleId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(r => r.Name, request.Name)
                .SetProperty(r => r.NormalizedName, request.Name.ToUpperInvariant())
                .SetProperty(r => r.Description, request.Description), ct);

        return rows == 1
            ? StatusCodes.Status200OK
            : (StatusCodes.Status404NotFound, "Role not found.");
    }

    public async Task<Outcome> DeleteAsync(Guid roleId, CancellationToken ct = default)
    {
        var rows = await _db.Roles
            .TagWith($"{nameof(RoleService)}.{nameof(DeleteAsync)}")
            .Where(r => r.Id == roleId)
            .ExecuteDeleteAsync(ct);

        return rows == 1
            ? StatusCodes.Status200OK
            : (StatusCodes.Status404NotFound, "Role not found.");
    }
}
