using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ScienceAtrium.Infrastructure.Extensions;
public static class DistributedCacheExtensions
{
    /// <summary>
    /// In seconds
    /// </summary>
    private const int DefaultTimeExpiration = 180;
    
    public static void SetRecord<TData>(
        this IDistributedCache cache,
        string key,
        TData data,
        JsonSerializerSettings? jsonSerializerSettings = null,
        TimeSpan? absoluteTimeExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null)
    {
        var options = GetEntryOptions(absoluteTimeExpirationRelativeToNow, slidingExpiration);

        var jsonData = GetSerializedData(data, jsonSerializerSettings);

        cache.SetString(key, jsonData, options);
    }

    public static async Task SetRecordAsync<TData>(
        this IDistributedCache cache,
        string key,
        TData data,
        JsonSerializerSettings? jsonSerializerSettings = null,
        TimeSpan? absoluteTimeExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null,
        CancellationToken cancellationToken = default)
    {
        var options = GetEntryOptions(absoluteTimeExpirationRelativeToNow, slidingExpiration);

        var jsonData = GetSerializedData(data, jsonSerializerSettings);

        await cache.SetStringAsync(key, jsonData, options, cancellationToken);
    }

    public static TData GetRecord<TData>(
        this IDistributedCache cache,
        string key,
        JsonSerializerSettings? jsonSerializerSettings = null)
    {
        var data = cache.GetString(key);

        if (data is null)
            return default;

        return JsonConvert.DeserializeObject<TData>(data, jsonSerializerSettings);
    }

    public static async Task<TData> GetRecordAsync<TData>(
        this IDistributedCache cache,
        string key,
        JsonSerializerSettings? jsonSerializerSettings = null,
        CancellationToken cancellationToken = default)
    {
        var data = await cache.GetStringAsync(key, cancellationToken);

        if (data is null)
            return default;

        return JsonConvert.DeserializeObject<TData>(data, jsonSerializerSettings);
    }

    private static DistributedCacheEntryOptions GetEntryOptions(
        TimeSpan? absoluteTimeExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null)
    {
        return new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteTimeExpirationRelativeToNow ??
                TimeSpan.FromSeconds(DefaultTimeExpiration),
            SlidingExpiration = slidingExpiration,
        };
    }

    private static string GetSerializedData<TData>(TData data, JsonSerializerSettings? jsonSerializerSettings = null)
    {
        return JsonConvert.SerializeObject(data, Formatting.None, jsonSerializerSettings ??
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
    }
}
