using Microsoft.Extensions.Configuration;
using MiniWebApp.AppHost.Options;

var builder = DistributedApplication.CreateBuilder(args);

AppHostOptions options = builder.Configuration
       .GetSection("AppHost")
       .Get<AppHostOptions>()!;

IResourceBuilder<RedisResource>? cache = null;
IResourceBuilder<PostgresServerResource>? postgres = null;
IResourceBuilder<PostgresDatabaseResource>? userdb = null;
IResourceBuilder<PostgresDatabaseResource>? taskdb = null;
IResourceBuilder<KafkaServerResource>? kafka = null;

//
// INFRASTRUCTURE
//

if (options.Infrastructure.Kafka)
{
    kafka = builder.AddKafka("kafka")
        .WithKafkaUI()
        .WithDataVolume()
        .WithLifetime(ContainerLifetime.Persistent);
}

if (options.Infrastructure.Redis)
{
    cache = builder.AddRedis("cache");
}

if (options.Infrastructure.Postgres)
{
    postgres = builder.AddPostgres("postgres");

    // Multiple databases from same server
    userdb = postgres.AddDatabase("userdb");
    taskdb = postgres.AddDatabase("taskdb");
}

if (options.Infrastructure.LocalStack)
{
    builder.AddContainer("localstack", "localstack/localstack")
        .WithHttpEndpoint(4566, 4566)
        .WithEnvironment("SERVICES", "s3")
        .WithEnvironment("DEBUG", "1");
}

//
// PROJECTS
//

if (options.Projects.UserApi)
{
    var userApi = builder.AddProject<Projects.MiniWebApp_UserApi>("miniwebapp-userapi");

    if (userdb is not null)
        userApi.WithReference(userdb).WaitFor(userdb);

    if (cache is not null)
        userApi.WithReference(cache).WaitFor(cache);

    if (kafka is not null)
        userApi.WithReference(kafka).WaitFor(kafka);
}

if (options.Projects.ApiService)
{
    var apiService = builder.AddProject<Projects.MiniWebApp_ApiService>("apiservice")
        .WithHttpHealthCheck("/health");

    if (cache is not null)
        apiService.WithReference(cache);

    if (taskdb is not null)
        apiService.WithReference(taskdb).WaitFor(taskdb);

    if (kafka is not null)
        apiService.WithReference(kafka).WaitFor(kafka);
}

if (options.Projects.WebFrontend)
{
    var web = builder.AddProject<Projects.MiniWebApp_Web>("webfrontend")
        .WithExternalHttpEndpoints()
        .WithHttpHealthCheck("/health");

    if (cache is not null)
        web.WithReference(cache).WaitFor(cache);
}

if (options.Projects.ReactApp)
{
    builder.AddJavaScriptApp("web", "../miniwebapp.reactapp", "dev")
        .WithNpm(true, "ci", ["--legacy-peer-deps"]);
}

if (options.Projects.TaskApi && taskdb is not null)
{
    builder.AddProject<Projects.MiniWebApp_TaskAPI>("miniwebapp-taskapi")
        .WithReference(taskdb)
        .WaitFor(taskdb);
}

if (options.Projects.KakfaApi && kafka is not null)
{
    builder.AddProject<Projects.KakfaApi>("kakfaapi")
        .WithReference(kafka)
        .WaitFor(kafka);
}

builder.Build().Run();
