using MiniWebApp.UserApi.Models.Permissions;

namespace MiniWebApp.UserApi.Services.Permissions;

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
