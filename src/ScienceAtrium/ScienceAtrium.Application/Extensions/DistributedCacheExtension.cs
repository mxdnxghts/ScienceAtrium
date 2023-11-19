using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ScienceAtrium.Application.Extensions;
public static class DistributedCacheExtensions
{
    /// <summary>
    /// In minutes
    /// </summary>
    private const int DefaultTimeExpiration = 20;

    public static async Task SetRecordAsync<TData>(
        this IDistributedCache cache,
        string key,
        TData data,
        TimeSpan? absoluteTimeExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null,
        CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteTimeExpirationRelativeToNow ??
                TimeSpan.FromMinutes(DefaultTimeExpiration),
            SlidingExpiration = slidingExpiration,
        };

        var jsonData = JsonConvert.SerializeObject(data, Formatting.None);
        await cache.SetStringAsync(key, jsonData, options, cancellationToken);
    }

    public static async Task<TData> GetRecordAsync<TData>(
        this IDistributedCache cache,
        string key,
        CancellationToken cancellationToken = default)
    {
        var data = await cache.GetStringAsync(key, cancellationToken);

        if (data is null)
            return default;

		return JsonConvert.DeserializeObject<TData>(data);
    }
}
