using MiniWebApp.UserApi.Contracts.Roles;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Models.Permissions;

namespace MiniWebApp.UserApi.Services.Repositories;

public sealed class PermissionQueries(
    UserDbContext db)
    : IPermissionQueries
{
    public async Task<Outcome<PermissionResponse>> GetPermissionAsync(
        GetPermissionRequest request,
        CancellationToken ct)
    {
        if (!request.Id.HasValue && string.IsNullOrWhiteSpace(request.Code))
        {
            return ("Provide an ID or Code to search.", StatusCodes.Status400BadRequest);
        }

        var query = db.Permissions.AsNoTracking();

        if (request.Id.HasValue && request.Id != Guid.Empty)
        {
            query = query.Where(x => x.Id == request.Id);
        }
        else if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var normalizedCode = request.Code.ToLowerInvariant().Trim();
            query = query.Where(x => x.Code == normalizedCode);
        }

        var result = await query
            .ProjectToResponse()
            .FirstOrDefaultAsync(ct);

        return result is null
            ? ("Permission not found.", StatusCodes.Status404NotFound)
            : (StatusCodes.Status200OK, result);
    }

    public async Task<Outcome<PermissionResponse[]>> ListPermissions(CancellationToken ct)
    {

        var items = await db.Permissions
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .ProjectToResponse()
            .ToArrayAsync(ct);

        return (StatusCodes.Status200OK, items);
    }
}
