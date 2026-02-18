using MiniWebApp.ApiService.Extensions;
using MiniWebApp.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddRedisClient(connectionName: "cache");

builder.Services.AddAwsS3Service(builder.Configuration);

var app = builder.Build();
var config = app.Configuration;


// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.MapGet("/", () => "API service is running.");



app.MapDefaultEndpoints();

await app.RunAsync();




