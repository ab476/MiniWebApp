using MemoryPack;
using Microsoft.Extensions.Caching.Distributed;
using MiniWebApp.UserApi.Models.Permissions;
using System.Text;

namespace MiniWebApp.UserApi.Services.Permissions;

public class DistributedPermissionQueriesDecorator(
    IPermissionQueries inner,
    IDistributedCache cache) : IPermissionQueries
{
    private const string CachePrefix = "perms";

    public async Task<Outcome<PermissionResponse>> GetPermissionAsync(
        GetPermissionRequest request,
        CancellationToken ct)
    {
        // 1. Generate a unique key based on the provided identifier
        // Example: "perm:id:guid-value" or "perm:code:roles.read"
        string identifier = request.Id.HasValue && request.Id != Guid.Empty
            ? $"id:{request.Id}"
            : $"code:{request.Code?.ToLowerInvariant().Trim()}";

        string key = $"{CachePrefix}:{identifier}";

        // 2. Check Cache
        byte[]? cachedBytes = await cache.GetAsync(key, ct);
        if (cachedBytes is not null)
        {
            var cachedResponse = MemoryPackSerializer.Deserialize<PermissionResponse>(cachedBytes);
            return (StatusCodes.Status200OK, cachedResponse!);
        }

        // 3. Fetch from DB (Inner Service)
        var result = await inner.GetPermissionAsync(request, ct);

        // 4. Cache the result if found
        if (result.IsSuccess)
        {
            byte[] data = MemoryPackSerializer.Serialize(result.Value);

            // Since permissions are "Read-Only" system data, 
            // a longer expiration (like 24h) is often safer than 1h.
            await cache.SetAsync(key, data, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                SlidingExpiration = TimeSpan.FromHours(4)
            }, ct);
        }

        return result;
    }

    public async Task<Outcome<PermissionResponse[]>> ListPermissions(CancellationToken ct)
    {
        string key = $"{CachePrefix}:list";

        byte[]? cachedBytes = await cache.GetAsync(key, ct);
        if (cachedBytes is not null)
        {
            var cachedPagedResponse = MemoryPackSerializer.Deserialize<PermissionResponse[]>(cachedBytes);
            return (StatusCodes.Status200OK, cachedPagedResponse!);
        }

        var result = await inner.ListPermissions(ct);

        if (result.IsSuccess)
        {
            byte[] data = MemoryPackSerializer.Serialize(result.Value);
            await cache.SetAsync(key, data, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            }, ct);
        }

        return result;
    }
}

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration, CancellationToken ct);

    // The "Magic" method that handles the pattern automatically
    Task<Outcome<T>> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<Outcome<T>>> factory,
        TimeSpan? expiration,
        CancellationToken ct);
}

public class MemoryPackCacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        var cachedBytes = await cache.GetAsync(key, ct);

        if (cachedBytes is null || cachedBytes.Length == 0)
        {
            return default;
        }

        // Special handling for strings (like your 'Version' key) 
        // to avoid MemoryPack overhead on primitive types
        if (typeof(T) == typeof(string))
        {
            return (T)(object)Encoding.UTF8.GetString(cachedBytes);
        }

        try
        {
            // Binary deserialization via MemoryPack
            return MemoryPackSerializer.Deserialize<T>(cachedBytes);
        }
        catch (Exception ex)
        {
            // Log the error (e.g., if the model changed and the binary format is incompatible)
            // Returning null allows the app to fallback to the database instead of crashing
            return default;
        }
    }

    // Complementary SetAsync to handle the binary conversion
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration, CancellationToken ct)
    {
        byte[] data;

        if (value is string s)
        {
            data = Encoding.UTF8.GetBytes(s);
        }
        else
        {
            data = MemoryPackSerializer.Serialize(value);
        }

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
        };

        await cache.SetAsync(key, data, options, ct);
    }

    public async Task<Outcome<T>> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<Outcome<T>>> factory,
        TimeSpan? expiration,
        CancellationToken ct)
    {
        // 1. Try Get
        var cachedBytes = await cache.GetAsync(key, ct);
        if (cachedBytes is not null)
        {
            return (StatusCodes.Status200OK, MemoryPackSerializer.Deserialize<T>(cachedBytes)!);
        }

        // 2. Cache Miss - Execute DB call
        var result = await factory(ct);

        // 3. Set if successful
        if (result.IsSuccess)
        {
            var data = MemoryPackSerializer.Serialize(result.Value);
            await cache.SetAsync(key, data, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
            }, ct);
        }

        return result;
    }

    
}