using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MiniWebApp.Core.Security;

public sealed class ConfigureJwtBearerOptions(IOptions<JwtOptions> jwtOptions)
        : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
            return;

        var key = new SymmetricSecurityKey(_jwtOptions.GetBytes());

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            IssuerSigningKey = key,

            ClockSkew = TimeSpan.Zero
        };
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(JwtBearerDefaults.AuthenticationScheme, options);
    }
}
