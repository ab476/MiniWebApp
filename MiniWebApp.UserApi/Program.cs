using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MiniWebApp.Core.Exceptions;
using MiniWebApp.Core.Security;
using MiniWebApp.ServiceDefaults;
using MiniWebApp.UserApi;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Infrastructure.HostedService;
using MiniWebApp.UserApi.Infrastructure.Serialization;
using MiniWebApp.UserApi.Services.Permissions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationAuthorization();

builder
    .AddDatabaseSeeding<TUser>()
    .AddSecurity()
    .AddCustomSerialization();



builder.Services.AddScoped<TenantService>().AddScoped<RoleService>()
    .AddScoped<IPermissionQueries, PermissionQueries>();
builder.AddNpgsqlDbContext<UserDbContext>("userdb");
builder.Services
    .AddValidatorsFromAssemblyContaining<Program>();



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(); // configuration happens via IConfigureOptions

builder.Services.AddAuthorization();
var app = builder.Build();

app.UseExceptionHandlingMiddleware();
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
