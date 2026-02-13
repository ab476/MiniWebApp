using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniWebApp.UserApi.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.MapDefaultEndpoints();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapSwagger().RequireAuthorization();
app.MapGet("/", () => "Hello World!");
app.MapGet("/db/script", ([FromServices] UserDbContext db) =>
{
    var script = db.Database.GenerateCreateScript();
    return Results.Text(script, "text/plain");
});
app.Run();
