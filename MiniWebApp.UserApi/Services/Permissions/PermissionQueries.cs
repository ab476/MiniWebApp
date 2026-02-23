using Microsoft.EntityFrameworkCore;
using MiniWebApp.Core.Common;
using MiniWebApp.Core.Models;
using MiniWebApp.UserApi.Contracts.Roles;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Models.Permissions;

namespace MiniWebApp.UserApi.Services.Permissions;

public sealed class PermissionQueries(
    UserDbContext db)
    : IPermissionQueries
{
    public async Task<Outcome<PermissionResponse>> GetPermissionAsync(
        GetPermissionRequest request,
        CancellationToken ct)
    {
        var query = db.TPermissions.AsNoTracking();

        // Dynamically filter based on what was provided
        if (request.Id.HasValue && request.Id != Guid.Empty)
        {
            query = query.Where(x => x.Id == request.Id);
        }
        else if (!string.IsNullOrWhiteSpace(request.Code))
        {
            // Normalize search to lowercase to match our DB constraints
            var normalizedCode = request.Code.ToLowerInvariant().Trim();
            query = query.Where(x => x.Code == normalizedCode);
        }

        var result = await query
            .ProjectToResponse()
            .FirstOrDefaultAsync(ct);

        if (result is null)
        {
            return ("Permission not found.", StatusCodes.Status404NotFound);
        }

        return (StatusCodes.Status200OK, result);
    }

    public async Task<Outcome<PagedResponse<PermissionResponse>>> GetPagedAsync(
        PagedRequest request,
        CancellationToken ct)
    {
        // 1. Get total count before slicing the data
        var totalCount = await db.TPermissions.CountAsync(ct);

        var (pageNumber, pageSize) = request;

        // 2. Apply Skip and Take for pagination
        var items = await db.TPermissions
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .Skip((pageNumber - 1) * pageSize) // Logic: (1-1)*10 = 0; (2-1)*10 = 10
            .Take(pageSize)
            .ProjectToResponse()
            .ToListAsync(ct);

        PagedResponse<PermissionResponse> pagedResult = new(
            items,
            totalCount,
            pageNumber,
            pageSize
        );

        return (StatusCodes.Status200OK, pagedResult);
    }
}
