var builder = DistributedApplication.CreateBuilder(args);
builder.AddRedis("cache").WithRedisInsight();
builder.Build().Run();
