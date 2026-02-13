var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var mysql = builder.AddMySql("mysql")
                .WithDataVolume("mysqldata")
                .WithLifetime(ContainerLifetime.Persistent)
                .WithPhpMyAdmin();

var mysqldb = mysql.AddDatabase("mysqldb");

var apiService = builder.AddProject<Projects.MiniWebApp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(mysqldb)
    .WithReference(mysqldb);

var localstack = builder.AddContainer("localstack", "localstack/localstack")
    .WithHttpEndpoint(port: 4566, targetPort: 4566)
    .WithEnvironment("SERVICES", "s3")   // optional: limit to S3
    .WithEnvironment("DEBUG", "1");

builder.AddProject<Projects.MiniWebApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

// React app
var react = builder.AddJavaScriptApp("web", "../miniwebapp.reactapp", "dev")
    .WithNpm(installCommand: "ci", installArgs: ["--legacy-peer-deps"]);
//.WithHttpEndpoint(port: 50568); // Vite default
//.WithEnvironment("VITE_API_BASE_URL", api.GetEndpoint("http");

builder.AddProject<Projects.MiniWebApp_TaskAPI>("miniwebapp-taskapi")
    .WithReference(mysqldb)
    .WaitFor(mysqldb);


builder.AddProject<Projects.MiniWebApp_UserApi>("miniwebapp-userapi");


builder.Build().Run();
