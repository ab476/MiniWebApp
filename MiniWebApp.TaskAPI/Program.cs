using MiniWebApp.TaskAPI.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddMySqlDataSource(connectionName: "mysqldb");
builder.AddMySqlDbContext<TaskAPIDbContext>(connectionName: "mysqldb");

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();
