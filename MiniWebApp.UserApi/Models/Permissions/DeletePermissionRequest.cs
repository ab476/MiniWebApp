namespace MiniWebApp.UserApi.Models.Permissions;

public sealed record DeletePermissionRequest
{
    public Guid Id { get; init; }
}
