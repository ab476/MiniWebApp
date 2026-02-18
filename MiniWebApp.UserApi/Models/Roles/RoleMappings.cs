using MiniWebApp.UserApi.Domain.Models;
using MiniWebApp.UserApi.Models.Roles;
using Riok.Mapperly.Abstractions;
using System.Security;

namespace MiniWebApp.UserApi.Contracts.Roles;

[Mapper]
public static partial class RoleMappings
{
    [MapperIgnoreSource(nameof(TRole.Tenant))]
    [MapperIgnoreSource(nameof(TRole.UserRoles))]
    [MapperIgnoreSource(nameof(TRole.RolePermissions))]
    public static partial RoleResponse ToResponse(this TRole role);

    public static partial IQueryable<RoleResponse> ProjectToResponse(this IQueryable<TRole> query);
}
