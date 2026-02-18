using MiniWebApp.UserApi.Domain.Models;
using MiniWebApp.UserApi.Models.Permissions;
using Riok.Mapperly.Abstractions;

namespace MiniWebApp.UserApi.Contracts.Roles;

[Mapper]
public static partial class PermissionMappings
{
    /// <summary>
    /// Maps a <see cref="Permission"/> domain entity
    /// to a <see cref="PermissionResponse"/> contract model.
    ///
    /// Navigation collections are ignored to prevent
    /// unnecessary data expansion.
    /// </summary>
    [MapperIgnoreSource(nameof(TPermission.RolePermissions))]
    public static partial PermissionResponse ToResponse(this TPermission permission);

    /// <summary>
    /// Projects an <see cref="IQueryable{Permission}"/> 
    /// directly to <see cref="PermissionResponse"/> using
    /// expression mapping (EF Core optimized).
    /// </summary>
    public static partial IQueryable<PermissionResponse> ProjectToResponse(
        this IQueryable<TPermission> query);
}
