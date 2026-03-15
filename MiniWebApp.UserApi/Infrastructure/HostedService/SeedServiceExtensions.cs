using Microsoft.AspNetCore.Identity;
using MiniWebApp.UserApi.Options;

namespace MiniWebApp.UserApi.Infrastructure.HostedService;

/// <summary>
/// Provides extension methods for registering database seeding services into the application's dependency injection container.
/// </summary>
public static class SeedServiceExtensions
{
    /// <summary>
    /// Configures and registers all necessary services, options, and hosted services required for the automated database seeding pipeline.
    /// </summary>
    /// <typeparam name="TUser">The type representing the user entity in the application.</typeparam>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to add the services to.</param>
    /// <returns>The modified <see cref="WebApplicationBuilder"/> for method chaining.</returns>
    /// <remarks>
    /// <strong>Registration Details:</strong>
    /// <list type="bullet">
    ///     <item><description>Loads the 'appsettings.seeddata.json' configuration file.</description></item>
    ///     <item><description>Binds the loaded configuration to <see cref="SeedDataOptions"/>.</description></item>
    ///     <item><description>Registers a generic <see cref="IPasswordHasher{TUser}"/> for secure credential generation.</description></item>
    ///     <item><description>Registers individual domain seeders (<see cref="PermissionSeeder"/>, <see cref="TenantSeeder"/>, <see cref="RoleSeeder"/>, <see cref="UserSeeder"/>) in execution order.</description></item>
    ///     <item><description>Registers the <see cref="ISystemIdentitySetupService"/> for establishing a super-admin context during execution.</description></item>
    ///     <item><description>Registers the <see cref="DatabaseSeederHostedService"/> as a background service to orchestrate the seeding process on startup.</description></item>
    /// </list>
    /// </remarks>
    public static WebApplicationBuilder AddDatabaseSeeding(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.seeddata.json", optional: true, reloadOnChange: true);

        builder.Services.Configure<SeedDataOptions>(
            builder.Configuration.GetSection("SeedData"));

        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        builder.Services.AddScoped<IDataSeeder, PermissionSeeder>();
        builder.Services.AddScoped<IDataSeeder, TenantSeeder>();
        builder.Services.AddScoped<IDataSeeder, RoleSeeder>();
        builder.Services.AddScoped<IDataSeeder, UserSeeder>();

        builder.Services.AddScoped<ISystemIdentitySetupService, SystemIdentitySetupService>();

        builder.Services.AddHostedService<DatabaseSeederHostedService>();

        return builder;
    }
}