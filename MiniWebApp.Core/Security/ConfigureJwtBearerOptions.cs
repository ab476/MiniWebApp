using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MiniWebApp.Core.Security;

/// <summary>
/// Configures the <see cref="JwtBearerOptions"/> using settings from <see cref="JwtOptions"/>.
/// <br/>
/// This implementation ensures strict validation and optimizes key allocation.
/// </summary>
public sealed class ConfigureJwtBearerOptions(IOptions<JwtOptions> jwtOptions)
        : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public void Configure(string? name, JwtBearerOptions options)
    {
        // Only configure if the scheme matches the default JWT scheme
        if (name != JwtBearerDefaults.AuthenticationScheme)
            return;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            // Security hardening: Ensure the token actually has an expiration claim
            RequireExpirationTime = true,

            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,

            // Cached key for performance
            IssuerSigningKey = new SymmetricSecurityKey(_jwtOptions.GetBytes()),

            // Eliminates the default 5-minute grace period for expired tokens
            ClockSkew = TimeSpan.Zero
        };
    }

    /// <summary>
    /// Fallback configuration for unnamed options.
    /// </summary>
    public void Configure(JwtBearerOptions options) =>
        Configure(JwtBearerDefaults.AuthenticationScheme, options);
}