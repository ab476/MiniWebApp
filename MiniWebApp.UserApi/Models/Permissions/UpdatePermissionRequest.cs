namespace MiniWebApp.UserApi.Models.Permissions;

public sealed record UpdatePermissionRequest
{
    public string Code { get; init; } = default!;
    public string? Description { get; init; }
    public string? Category { get; init; }
}
