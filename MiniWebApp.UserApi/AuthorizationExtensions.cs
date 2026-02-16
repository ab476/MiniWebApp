using MiniWebApp.Core.Auth;

namespace MiniWebApp.UserApi;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddApplicationAuthorization(
       this IServiceCollection services)
    {
        var builder = services.AddAuthorizationBuilder();

        // Register permission policies
        foreach (var permission in AppPermissions.All)
        {
            builder.AddPolicy(permission,
                policy => policy.RequireClaim(AppClaimTypes.Permissions, permission)
            );
        }

        // Register role policies
        foreach (var role in AppRoles.All)
        {
            builder.AddPolicy(role, policy => policy.RequireRole(role));
        }

        return services;
    }
}
