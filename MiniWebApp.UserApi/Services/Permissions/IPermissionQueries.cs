using MiniWebApp.Core.Common;
using MiniWebApp.Core.Models;
using MiniWebApp.UserApi.Models.Permissions;

namespace MiniWebApp.UserApi.Services.Permissions;

public interface IPermissionQueries
{
    Task<Outcome<PermissionResponse>> GetPermissionAsync(
        GetPermissionRequest request,
        CancellationToken ct
    );

    Task<Outcome<PagedResponse<PermissionResponse>>> GetPagedAsync(
        PagedRequest request,
        CancellationToken ct
    );
}
