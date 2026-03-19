using MiniWebApp.UserApi.Infrastructure;
using MiniWebApp.UserApi.Services.Repositories;
using System.Linq.Expressions;

namespace MiniWebApp.UserApi.Models.Tenants;

/// <summary>
/// Provides mapping extensions for <see cref="Tenant"/> entities to <see cref="TenantResponse"/> DTOs.
/// </summary>
public static class TenantMapperExtensions
{
    /// <summary>
    /// Defines the expression for mapping a <see cref="Tenant"/> to a <see cref="TenantResponse"/>.
    /// </summary>
    private static readonly Expression<Func<Tenant, TenantResponse>> MappingExpression =
        tenant => new TenantResponse(
            Id: tenant.Id,
            Name: tenant.Name,
            Domain: tenant.Domain,
            IsActive: tenant.IsActive,
            CreatedAt: tenant.CreatedAt,
            UpdatedAt: tenant.UpdatedAt
        );

    /// <summary>
    /// A compiled function for mapping a single <see cref="Tenant"/> to a <see cref="TenantResponse"/>.
    /// </summary>
    private static readonly Func<Tenant, TenantResponse> CompiledMapper =
        MappingExpression.Compile();

    /// <summary>
    /// Projects an <see cref="IQueryable{Tenant}"/> to an <see cref="IQueryable{TenantResponse}"/> (EF Core optimized).
    /// </summary>
    /// <param name="query">The original queryable of Tenant entities.</param>
    /// <returns>A queryable of TenantResponse DTOs.</returns>
    public static IQueryable<TenantResponse> ProjectToResponse(this IQueryable<Tenant> query)
    {
        return query.Select(MappingExpression);
    }

    /// <summary>
    /// Converts a single <see cref="Tenant"/> entity to a <see cref="TenantResponse"/>.
    /// </summary>
    /// <param name="tenant">The Tenant entity to convert.</param>
    /// <returns>A TenantResponse DTO.</returns>
    public static TenantResponse ToResponse(this Tenant tenant)
    {
        return CompiledMapper.Invoke(tenant);
    }
    /// <summary>
    /// Maps a <see cref="CreateTenantRequest"/> to a new <see cref="Tenant"/> entity.
    /// </summary>
    public static Tenant ToEntity(this CreateTenantRequest request, DateTime timeStamp)
    {
        return new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Domain = request.Domain,
            IsActive = true,
            CreatedAt = timeStamp
        };
    }
    public static IAuditTask ToAuditRequest(this IEnumerable<Tenant> tenants, AuditAction action, Guid? userId, DateTime timestamp)
    {
        return AuditTask.Create(
            entity: tenants.Select(tenant => new AuditTenantRequest(
                TenantId: tenant.Id,
                Name: tenant.Name,
                Domain: tenant.Domain,
                IsActive: tenant.IsActive
            )).ToArray(),
            action: action,
            userId: userId,
            timestamp: timestamp);
    }
    public static IAuditTask ToAuditRequest(this Tenant tenant, AuditAction action, Guid? userId)
    {
        return AuditTask.Create(
            entity: new AuditTenantRequest(
                TenantId: tenant.Id,
                Name: tenant.Name,
                Domain: tenant.Domain,
                IsActive: tenant.IsActive
            ),  
            action: action,
            userId: userId,
            timestamp: tenant.UpdatedAt ?? tenant.CreatedAt);
    }
}

/// <summary>
/// Provides mapping extensions for <see cref="User"/> entities to <see cref="UserResponse"/> DTOs.
/// </summary>
public static class UserMapperExtensions
{
    /// <summary>
    /// Defines the expression for mapping a <see cref="User"/> to a <see cref="UserResponse"/>.
    /// </summary>
    private static readonly Expression<Func<User, UserResponse>> MappingExpression =
        user => new UserResponse(
            Id: user.Id,
            Email: user.Email,
            UserName: user.UserName,
            Status: user.Status,
            CreatedAt: user.CreatedAt,
            TenantId: user.TenantId
        );

    /// <summary>
    /// A compiled function for mapping a single <see cref="User"/> to a <see cref="UserResponse"/>.
    /// </summary>
    private static readonly Func<User, UserResponse> CompiledMapper =
        MappingExpression.Compile();

    /// <summary>
    /// Projects an <see cref="IQueryable{User}"/> to an <see cref="IQueryable{UserResponse}"/> (EF Core optimized).
    /// </summary>
    /// <param name="query">The original queryable of User entities.</param>
    /// <returns>A queryable of UserResponse DTOs.</returns>
    public static IQueryable<UserResponse> ProjectToResponse(this IQueryable<User> query)
    {
        return query.Select(MappingExpression);
    }

    /// <summary>
    /// Converts a single <see cref="User"/> entity to a <see cref="UserResponse"/>.
    /// </summary>
    /// <param name="user">The User entity to convert.</param>
    /// <returns>A UserResponse DTO.</returns>
    public static UserResponse ToResponse(this User user)
    {
        return CompiledMapper.Invoke(user);
    }
}