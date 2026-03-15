using Microsoft.Extensions.DependencyInjection;
using MiniWebApp.Core.Services;

namespace MiniWebApp.Core.Extensions;

/// <summary>
/// Contains extension methods for registering core application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <see cref="IScopedStateService"/> with a <c>Scoped</c> lifetime.
    /// <br/>
    /// <br/>
    /// <b>Lifetime:</b> Scoped
    /// <br/>
    /// The service will be created once per client request (or per Blazor circuit).
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for method chaining.</returns>
    public static IServiceCollection AddScopedState(this IServiceCollection services)
    {
        // We register it as Scoped because the state is intended 
        // to persist throughout a single request/session.
        services.AddScoped<IScopedStateService, ScopedStateService>();

        return services;
    }
}