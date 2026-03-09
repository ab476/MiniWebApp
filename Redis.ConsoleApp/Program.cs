using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

// --- Configure Logging Level in Code ---
builder.Logging.SetMinimumLevel(LogLevel.None);
// Optional: Filter specific categories
//builder.Logging.AddFilter("Microsoft", LogLevel.None);
//builder.Logging.AddFilter("Aspire", LogLevel.None);

// 1. Add the Redis client using the connection name defined in your AppHost
builder.AddRedisClient(connectionName: "cache");

using IHost host = builder.Build();

// 2. Resolve the multiplexer from the DI container
var redis = host.Services.GetRequiredService<IConnectionMultiplexer>();
var db = redis.GetDatabase();
Console.WriteLine("---------------------------------------------------------------");
Console.WriteLine("Connected to Redis!");

RedisKey instructorKey = "instructor:1";
await db.StringSetAsync(instructorKey, "John Doe");

string? instructor1Name = await db.StringGetAsync(instructorKey);
Console.WriteLine($"Instructor 1's name is: {instructor1Name}");
Console.WriteLine();

var tempKey = "temporary:key";

await db.StringSetAsync(tempKey, 42, when: When.NotExists);
await db.StringIncrementAsync(tempKey, 8);

Console.WriteLine("Temporary key value after increment: " + await db.StringGetAsync(tempKey));


