using MiniWebApp.UserApi.Models.Permissions;

namespace MiniWebApp.UserApi.Services.Repositories;

public interface IPermissionQueries
{
    Task<Outcome<PermissionResponse>> GetPermissionAsync(
        GetPermissionRequest request,
        CancellationToken ct
    );

    Task<Outcome<PermissionResponse[]>> ListPermissions(
        CancellationToken ct
    );
}
