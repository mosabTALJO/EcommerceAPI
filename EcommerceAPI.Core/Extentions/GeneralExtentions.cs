using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPI.Core.Extentions
{
    public static class GeneralExtentions
    {
        public static async Task<T> GetValueFromCache<T>(this IMemoryCache memoryCache, string Key, Func<Task<T>> action, DateTime? expiration = null)
        {
            if (memoryCache.TryGetValue(Key, out T result))
                return result;
            if (action is null)
                return default;

            result = await action.Invoke();
            // setting cache entries with options
            memoryCache.Set(Key, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = expiration ?? DateTime.Now.AddSeconds(10),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromHours(1)
            });

            return result;
        }
    }
}