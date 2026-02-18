using Microsoft.EntityFrameworkCore;
using MiniWebApp.Core.Common;
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
        var exists = await db.TPermissions
            .AnyAsync(x => x.Code == request.Code, ct);

        if (exists)
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

        return Result.Success(Map(entity));
    }

    public async Task<Result<PermissionResponse>> GetByIdAsync(
        Guid id,
        CancellationToken ct)
    {
        var entity = await db.TPermissions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity is null)
            return Result.Failure<PermissionResponse>("Permission not found.");

        return Result.Success(Map(entity));
    }

    public async Task<Result<IReadOnlyList<PermissionResponse>>> GetAllAsync(
        CancellationToken ct)
    {
        var list = await db.TPermissions
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .Select(x => new PermissionResponse
            {
                Id = x.Id,
                Code = x.Code,
                Description = x.Description,
                Category = x.Category
            })
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<PermissionResponse>>(list);
    }

    public async Task<Result<PermissionResponse>> UpdateAsync(
        Guid id,
        UpdatePermissionRequest request,
        CancellationToken ct)
    {
        var entity = await db.TPermissions
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity is null)
            return Result.Failure<PermissionResponse>("Permission not found.");

        var duplicate = await db.TPermissions
            .AnyAsync(x => x.Code == request.Code && x.Id != id, ct);

        if (duplicate)
            return Result.Failure<PermissionResponse>("Permission code already exists.");

        entity.Code = request.Code;
        entity.Description = request.Description;
        entity.Category = request.Category;

        await db.SaveChangesAsync(ct);

        return Result.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(
        Guid id,
        CancellationToken ct)
    {
        var entity = await db.TPermissions
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity is null)
            return Result.Failure("Permission not found.");

        db.TPermissions.Remove(entity);
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }

    // ========================================================
    // MAPPING
    // ========================================================

    private static PermissionResponse Map(TPermission entity)
        => new()
        {
            Id = entity.Id,
            Code = entity.Code,
            Description = entity.Description,
            Category = entity.Category
        };
}
