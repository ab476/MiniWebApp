using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using MiniWebApp.Core.Exceptions;
using MiniWebApp.Core.Security;
using MiniWebApp.ServiceDefaults;
using MiniWebApp.UserApi;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Infrastructure;
using MiniWebApp.UserApi.Infrastructure.HostedService;
using MiniWebApp.UserApi.Infrastructure.Serialization;
using MiniWebApp.UserApi.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationAuthorization();

builder
    .AddDatabaseSeeding<User>()
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

builder.Services.AddScoped<ITenantRepository, TenantRepository>().AddScoped<RoleService>()
    .AddScoped<IPermissionQueries, PermissionQueries>();
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
