using MiniWebApp.UserApi.Domain.Models;
using Riok.Mapperly.Abstractions;

namespace MiniWebApp.UserApi.Contracts.Roles;

[Mapper]
public static partial class RoleMappings
{
    [MapperIgnoreSource(nameof(Role.Tenant))]
    [MapperIgnoreSource(nameof(Role.UserRoles))]
    [MapperIgnoreSource(nameof(Role.RolePermissions))]
    public static partial RoleResponse ToResponse(this Role role);

    public static partial IQueryable<RoleResponse> ProjectToResponse(this IQueryable<Role> query);
}
