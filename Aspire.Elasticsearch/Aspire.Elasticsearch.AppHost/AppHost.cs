var builder = DistributedApplication.CreateBuilder(args);

var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("elasticsearch")
    .WithEnvironment("http.cors.enabled", "true")
    .WithEnvironment("http.cors.allow-origin", "http://localhost:54735")
    .WithEnvironment("http.cors.allow-headers", "X-Requested-With,Content-Type,Content-Length,Authorization");

elasticsearch.WithElasticvue();

builder.Build().Run();
