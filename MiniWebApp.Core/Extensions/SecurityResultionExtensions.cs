using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniWebApp.Core.Security;

namespace MiniWebApp.Core.Extensions;

/// <summary>
/// Extension methods for setting up security and authentication.
/// </summary>
public static class SecurityResultionExtensions
{
    /// <summary>
    /// Registers JWT Authentication and configures options via <see cref="ConfigureJwtBearerOptions"/>.
    /// <br/>
    /// Expects a configuration section named "Jwt".
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        services.ConfigureOptions<ConfigureJwtBearerOptions>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer();

        return services;
    }
}