var builder = DistributedApplication.CreateBuilder(args);
var cache = builder.AddRedis("cache").WithRedisInsight();

builder.AddProject<Projects.Redis_ConsoleApp>("RedisConsoleApp")
    .WithReference(cache);

builder.Build().Run();
