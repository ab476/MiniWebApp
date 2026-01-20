var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Serve static files from wwwroot
app.UseDefaultFiles();   // allows index.html as default
app.UseStaticFiles();

// Example API endpoint
app.MapGet("/api/hello", () => new { message = "Hello from ASP.NET Core API" });
//app.MapGet("/", () => "Hello World!");

app.Run();
