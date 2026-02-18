namespace MiniWebApp.UserApi.Models.Permissions;

public sealed record GetPermissionByIdRequest
{
    public Guid Id { get; init; }
}
