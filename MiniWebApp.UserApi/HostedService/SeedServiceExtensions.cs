using Microsoft.AspNetCore.Identity;
using MiniWebApp.UserApi.Options;

namespace MiniWebApp.UserApi.HostedService;

public static class SeedServiceExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder AddDatabaseSeeding<TUser>()
        where TUser : class
        {
            // 1. Add the JSON file
            builder.Configuration.AddJsonFile("appsettings.seeddata.json", optional: true, reloadOnChange: true);

            // 2. Bind Options
            builder.Services.Configure<SeedDataOptions>(
                builder.Configuration.GetSection("SeedData"));

            // 3. Register Identity and Seeding Services
            builder.Services.AddScoped<IPasswordHasher<TUser>, PasswordHasher<TUser>>();
            builder.Services.AddScoped<IPermissionSeeder, PermissionSeeder>();
            builder.Services.AddScoped<ITenantSeeder, TenantSeeder>();
            builder.Services.AddScoped<IRoleSeeder, RoleSeeder>();
            builder.Services.AddScoped<IUserSeeder, UserSeeder>();

            // 4. Register the Background Service
            builder.Services.AddHostedService<DatabaseSeederHostedService>();

            return builder;
        }
    }
}