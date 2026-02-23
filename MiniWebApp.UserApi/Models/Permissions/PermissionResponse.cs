using MemoryPack;

namespace MiniWebApp.UserApi.Models.Permissions;

// ============================================================
// RESPONSE MODEL
// ============================================================

[MemoryPackable]
public sealed partial  record PermissionResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = default!;
    public string? Description { get; init; }
    public string? Category { get; init; }
}
