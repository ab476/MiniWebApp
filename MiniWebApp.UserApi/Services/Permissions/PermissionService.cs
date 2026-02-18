using Microsoft.EntityFrameworkCore;
using MiniWebApp.Core.Common;
using MiniWebApp.UserApi.Contracts.Roles;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Domain.Models;
using MiniWebApp.UserApi.Models.Permissions;

namespace MiniWebApp.UserApi.Application.Permissions;

// ============================================================
// SERVICE CONTRACT
// ============================================================

public interface IPermissionService
{
    Task<Outcome<PermissionResponse>> CreateAsync(
        CreatePermissionRequest request,
        CancellationToken ct);

    Task<Outcome<PermissionResponse>> GetByIdAsync(
        Guid id,
        CancellationToken ct);

    Task<Outcome<IReadOnlyList<PermissionResponse>>> GetAllAsync(
        CancellationToken ct);

    Task<Outcome<PermissionResponse>> UpdateAsync(
        Guid id,
        UpdatePermissionRequest request,
        CancellationToken ct);

    Task<Outcome> DeleteAsync(
        Guid id,
        CancellationToken ct);
}

// ============================================================
// SERVICE IMPLEMENTATION
// ============================================================

internal sealed class PermissionService(
    UserDbContext db)
    : IPermissionService
{
    public async Task<Outcome<PermissionResponse>> CreateAsync(
        CreatePermissionRequest request,
        CancellationToken ct)
    {

        if (await db.TPermissions.AnyAsync(x => x.Code == request.Code, ct))
            return ("Permission code already exists.", StatusCodes.Status409Conflict);

        var entity = new TPermission
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Description = request.Description,
            Category = request.Category
        };

        db.TPermissions.Add(entity);
        await db.SaveChangesAsync(ct);

        return (StatusCodes.Status201Created, entity.ToResponse());
    }

    public async Task<Outcome<PermissionResponse>> GetByIdAsync(
        Guid id,
        CancellationToken ct)
    {
        var entity = await db.TPermissions
            .AsNoTracking()
            .ProjectToResponse()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity is null)
            return ("Permission not found.", StatusCodes.Status404NotFound);

        return (StatusCodes.Status200OK, entity);
    }

    public async Task<Outcome<IReadOnlyList<PermissionResponse>>> GetAllAsync(
        CancellationToken ct)
    {
        var list = await db.TPermissions
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .ProjectToResponse()
            .ToListAsync(ct);

        return (StatusCodes.Status200OK, list);
    }

    public async Task<Outcome<PermissionResponse>> UpdateAsync(
        Guid id,
        UpdatePermissionRequest request,
        CancellationToken ct)
    {
        var entity = await db.TPermissions
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity is null)
            return ("Permission not found.", StatusCodes.Status404NotFound);

        var duplicate = await db.TPermissions
            .AnyAsync(x => x.Code == request.Code && x.Id != id, ct);

        if (duplicate)
            return ("Permission code already exists.", StatusCodes.Status409Conflict);

        entity.Code = request.Code;
        entity.Description = request.Description;
        entity.Category = request.Category;

        await db.SaveChangesAsync(ct);

        return (StatusCodes.Status200OK, entity.ToResponse());
    }

    public async Task<Outcome> DeleteAsync(
        Guid id,
        CancellationToken ct)
    {
        var entity = await db.TPermissions
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity is null)
            return ("Permission not found.", StatusCodes.Status404NotFound);

        db.TPermissions.Remove(entity);
        await db.SaveChangesAsync(ct);

        return StatusCodes.Status204NoContent;
    }

    
}
