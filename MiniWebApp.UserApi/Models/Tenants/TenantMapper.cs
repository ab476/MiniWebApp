using MiniWebApp.UserApi.Domain.Models;
using MiniWebApp.UserApi.Models.Tenants;
using Riok.Mapperly.Abstractions;

namespace MiniWebApp.UserApi.Contracts.Tenants;

[Mapper]
public static partial class TenantMapper
{
    public static partial TenantResponse ToResponse(this TTenant tenant);
    public static partial IQueryable<TenantResponse> ProjectToResponse(this IQueryable<TTenant> query);

    public static partial TTenant ToEntity(this TenantResponse response);
}