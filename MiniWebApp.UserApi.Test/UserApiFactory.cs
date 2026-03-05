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
        builder.UseSetting("ConnectionStrings:userdb", _connectionString);
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
        builder.UseSetting("ConnectionStrings:userdb", _connectionString);
        builder.UseEnvironment("Testing");
    }
}

internal class TestJwtSettings : IOptions<JwtOptions>
{

    private readonly JwtOptions jwtOptions = new ()
    {
        Issuer = "MiniWebApp",
        Audience = "MiniWebAppUsers",
        Key = "14f8176424f00751933b45b288038536598ace738ac1ba2a4f97717a1f21990522d5a031a9f9473ee80ddbf3d22be42d088ad4fbf50b7a58709c114f7371f9f1",
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