namespace MiniWebApp.UserApi.Models.Permissions;

public sealed record GetPermissionRequest
{
    public Guid? Id { get; init; }
    public string? Code { get; init; }
}
