using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EcommerceAPI.Services
{
    public interface ICurrencyRateService : IService<CurrencyRate>
    {
        double GetCurrencyRate(string from, string to);
        Task ClearCache();
        static string[] CurrencyCodes => ["QAR", "KWD", "USD", "TRY", "OMR", "AED", "YER", "EGP", "SAR", "IQD", "JOD", "BHD", "MAD", "DZD", "LYD",];
    }

    public class CurrencyRateService : Service<CurrencyRate>, ICurrencyRateService
    {
        readonly IMemoryCache _cache;
        public CurrencyRateService(IRepository<CurrencyRate> repo, IMemoryCache cache) : base(repo)
        {
            _cache = cache;
        }

        public double GetCurrencyRate(string from, string to)
        {
            var key = $"CC_{from}_{to}";
            if (!_cache.TryGetValue(key, out double rateValue))
            {
                var rateRecord = Repository.Queryable().FirstOrDefault(x => x.FromCurrencyCode == from && x.ToCurrencyCode == to);
                rateValue = rateRecord == null ? 0 : rateRecord.Rate;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(1));

                _cache.Set(key, rateValue, cacheEntryOptions);
            }

            return rateValue;
        }

        public async Task ClearCache()
        {
            var items = await Repository.Queryable().ToArrayAsync();
            foreach (var item in items)
                _cache.Remove($"CC_{item.FromCurrencyCode}_{item.ToCurrencyCode}");
        }
    }
}
