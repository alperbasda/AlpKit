using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AlpKit.Caching.Helpers;

public interface ICacheHelper
{
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpirationRelativeToNow = null);

    Task<T?> GetAsync<T>(string key);

    Task RemoveAsync(string key);
}
public class CacheHelper : ICacheHelper
{
    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _jsonOptions;

    public CacheHelper(IDistributedCache cache)
    {
        _cache = cache;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpirationRelativeToNow = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(30),
            SlidingExpiration = slidingExpirationRelativeToNow ?? TimeSpan.FromMinutes(15)
        };

        var jsonData = JsonSerializer.Serialize(value, _jsonOptions);
        await _cache.SetStringAsync(key, jsonData, options);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var jsonData = await _cache.GetStringAsync(key);

        if (jsonData == null)
            return default;

        return JsonSerializer.Deserialize<T>(jsonData, _jsonOptions);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

}
