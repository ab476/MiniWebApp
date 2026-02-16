using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniWebApp.Core.Exceptions;
using MiniWebApp.Core.Security;
using MiniWebApp.UserApi;
using MiniWebApp.UserApi.Application;
using MiniWebApp.UserApi.Application.Tenants;
using MiniWebApp.UserApi.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationAuthorization();


builder.Services.AddScoped<TenantService>().AddScoped<RoleService>();
builder.AddNpgsqlDbContext<UserDbContext>("userdb");
builder.Services
    .AddValidatorsFromAssemblyContaining<Program>();

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(); // configuration happens via IConfigureOptions
builder.Services.Configure<ConfigureJwtBearerOptions>(builder.Configuration);
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
