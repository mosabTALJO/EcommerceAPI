using EcommerceAPI.Shared.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace EcommerceAPI.Core.Extentions
{
    public static class GeneralExtentions
    {
        public static async Task<T?> GetValueFromCache<T>(this IMemoryCache memoryCache, string Key, Func<Task<T>> action, DateTime? expiration = null)
        {
            if (memoryCache.TryGetValue(Key, out T? result))
                return result;
            if (action is null)
                return default;

            result = await action.Invoke();
            // setting cache entries with options
            memoryCache.Set(Key, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = expiration ?? DateTime.UtcNow.AddSeconds(10),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromHours(1)
            });

            return result;
        }
        public static string GetPolicyNumber(this Policies p) => ((int)p).ToString();
        public static int PagingNumber(this int i) => i - 1;
    }
}