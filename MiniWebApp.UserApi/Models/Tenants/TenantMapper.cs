using MiniWebApp.UserApi.Domain.Models;
using MiniWebApp.UserApi.Models.Tenants;
using Riok.Mapperly.Abstractions;

namespace MiniWebApp.UserApi.Contracts.Tenants;

[Mapper]
public static partial class TenantMapper
{
    public static partial TenantResponse ToResponse(this Tenant tenant);
    public static partial IQueryable<TenantResponse> ProjectToResponse(this IQueryable<Tenant> query);

    public static partial Tenant ToEntity(this TenantResponse response);
}