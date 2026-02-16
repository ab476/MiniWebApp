using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MiniWebApp.Core.Security;

namespace MiniWebApp.UserApi.Test;

public class UserApiFactory(string connectionString, IOptions<JwtOptions> jwtSettings) : WebApplicationFactory<Program>
{
    private readonly string _connectionString = connectionString;
    public  readonly IOptions<JwtOptions> JwtSettings = jwtSettings;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var jwtSettings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:userdb"] = _connectionString,
                ["Jwt:Issuer"] = JwtSettings.Value.Issuer,
                ["Jwt:Audience"] = JwtSettings.Value.Audience,
                ["Jwt:Key"] = JwtSettings.Value.Key,
                ["Jwt:ExpiryMinutes"] = JwtSettings.Value.ExpiryMinutes.ToString()  
            };

            config.AddInMemoryCollection(jwtSettings);
        });

        builder.UseEnvironment("Testing");
    }
}

internal class TestJwtSettings : IOptions<JwtOptions>
{

    private readonly JwtOptions jwtOptions = new ()
    {
        Issuer = "MiniWebApp",
        Audience = "MiniWebAppUsers",
        Key = "qZrJk0rJkqYl9h0rY2fQmGq7l6lHc3gk3xv8bV2sYqTt6yP0nEwW5fKc4uRz8mHq1yLx2pQv7nM5aKc9tYgJw==",
        ExpiryMinutes = 60
    };
    public JwtOptions Value
    {
        get
        {
            return jwtOptions;
        }
    }
}