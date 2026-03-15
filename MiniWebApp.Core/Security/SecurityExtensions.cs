using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniWebApp.Core.Services;

namespace MiniWebApp.Core.Security;

/// <summary>
/// Provides extension methods for setting up security-related services in an <see cref="IHostApplicationBuilder"/>.
/// </summary>
public static class SecurityExtensions
{
    /// <summary>
    /// Registers authentication, authorization, and identity-related services.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining further service registrations.</returns>
    /// <remarks>
    /// This method configures:
    /// <list type="bullet">
    /// <item><description>JWT Options from the configuration section.</description></item>
    /// <item><description>HTTP Context Accessor for accessing the current user.</description></item>
    /// <item><description>Token generation and User context services.</description></item>
    /// </list>
    /// </remarks>
    public static IServiceCollection AddSecurity(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        // Binds JwtOptions from the appsettings.json section (e.g., "JwtSettings")
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        // Registers the logic to configure JwtBearerOptions (e.g., Validation parameters)
        services.ConfigureOptions<ConfigureJwtBearerOptions>();

        // Provides access to the HttpContext in services where it isn't automatically available
        services.AddHttpContextAccessor();

        // Singleton: The generator doesn't hold state, so one instance is enough for the app lifetime
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        // Scoped: Created once per client request
        services
            .AddScoped<IScopedStateService, ScopedStateService>()
            .AddScoped<IUserContext, UserContext>()
            .AddScoped<ITenantProvider>(sp => sp.GetRequiredService<IUserContext>());

        return services;
    }
}