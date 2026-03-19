using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniWebApp.Core.Security;
using MiniWebApp.UserApi.Services.Auth;
using MiniWebApp.UserApi.Services.Repositories;

namespace MiniWebApp.UserApi.Services;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services)
    {
        // Auth Logic
        services.TryAddScoped<IAuthService, AuthService>();
        services.TryAddScoped<IRefreshTokenService, RefreshTokenService>();
        services.TryAddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // Repositories (State changes)
        services.TryAddScoped<IUserRepository, UserRepository>();
        services.TryAddScoped<ILoginHistoryRepository, LoginHistoryRepository>();

        // Queries (Read-only data)
        services.TryAddScoped<IUserQueries, UserQueries>();
        services.TryAddScoped<IUserRoleQueries, UserRoleQueries>();
        services.TryAddScoped<IRoleClaimQueries, RoleClaimQueries>();

        return services;
    }
}