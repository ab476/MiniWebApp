using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi;
using MiniWebApp.Core.Exceptions;
using MiniWebApp.Core.Security;
using MiniWebApp.ServiceDefaults;
using MiniWebApp.UserApi;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Infrastructure;
using MiniWebApp.UserApi.Infrastructure.HostedService;
using MiniWebApp.UserApi.Infrastructure.Serialization;
using MiniWebApp.UserApi.Services;
using MiniWebApp.UserApi.Services.Auth;
using MiniWebApp.UserApi.Services.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
// Program.cs
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelAttribute>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddApplicationAuthorization();

builder
    .AddDatabaseSeeding()
    .AddSecurity()
    .AddCustomSerialization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IClientInfoProvider, ClientInfoProvider>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Clear default loopback limits for Docker/Reverse Proxy setups
    options.KnownProxies.Clear();
    options.KnownIPNetworks.Clear();
});

builder.Services
    .AddScoped<IRequestContext, OperationalContext>()
    .AddScoped<ITenantRepository, TenantRepository>()
    .AddScoped<ITenantQueries, TenantQueries>()
    .AddScoped<IRoleRepository, RoleRepository>()
    .AddScoped<IRoleQueries, RoleQueries>()
    .AddScoped<IUserRoleRepository, UserRoleRepository>()
    .AddScoped<IClaimRepository, ClaimRepository>()
    .AddScoped<IUserRoleQueries, UserRoleQueries>()
    .AddScoped<IRoleClaimRepository, RoleClaimRepository>()
    .AddScoped<IRoleQueries, RoleQueries>()
    .AddScoped<IClaimQueries, ClaimQueries>()
    .AddScoped<IUserRepository, UserRepository>()
    .AddScoped<IUserQueries, UserQueries>()
    .AddScoped<IAuthService, AuthService>()
    .AddScoped<IRefreshTokenService, RefreshTokenService>()

    .AddAuthInfrastructure();
builder.Services.AddSingleton<IAuditChannel, AuditChannel>();
builder.AddNpgsqlDbContext<UserDbContext>("userdb");
builder.Services
    .AddValidatorsFromAssemblyContaining<Program>();



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(); // configuration happens via IConfigureOptions

builder.Services.AddAuthorization();
var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseForwardedHeaders();
app.MapDefaultEndpoints();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "This is my Scalar API";
        options.DarkMode = true;
        options.Favicon = "path";
        options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.CSharp, ScalarClient.RestSharp);
        options.HideModels = false;
        options.Layout = ScalarLayout.Modern;
        options.ShowSidebar = true;

        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecuritySchemes = [JwtBearerDefaults.AuthenticationScheme]
        };
    });
}
////app.UseAuthorization();
//app.MapSwagger().RequireAuthorization();
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.MapGet("/db/script", ([FromServices] UserDbContext db) =>
{
    var script = db.Database.GenerateCreateScript();
    return Results.Text(script, "text/plain");
});
app.MapControllers();
app.Run();
