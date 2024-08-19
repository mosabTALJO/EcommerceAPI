using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.EF;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Scrapper;

namespace EcommerceAPI.Services
{
    public interface ICurrencyRateService : IService<CurrencyRate>
    {
        double GetCurrencyRate(string from, string to);
        double GetUSDRate(string from);
        Task RefreshRates();
        Task ClearCache();
        static string[] CurrencyCodes => ["QAR", "KWD", "USD", "TRY", "OMR", "AED", "YER", "EGP", "SAR", "IQD", "JOD", "BHD", "MAD", "DZD", "LYD",];
    }

    public class CurrencyRateService : Service<CurrencyRate>, ICurrencyRateService
    {
        readonly IMemoryCache _cache;
        readonly IUnitOfWork _unitOfWork;
        public CurrencyRateService(IRepository<CurrencyRate> repo, IMemoryCache cache, IUnitOfWork unitOfWork) : base(repo)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
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

        public async Task RefreshRates()
        {
            List<CurrencyRate> allItems = await CurrencyScrapper.Scrap(ICurrencyRateService.CurrencyCodes);
            var dbItems = Repository.Queryable().ToArray();
            foreach (var dbItem in dbItems)
            {
                var newItem = allItems.FirstOrDefault(x => x.FromCurrencyCode == dbItem.FromCurrencyCode && x.ToCurrencyCode == dbItem.ToCurrencyCode);
                if (newItem != null)
                {
                    dbItem.Rate = newItem.Rate;
                    Repository.Update(dbItem);
                    allItems.Remove(newItem);
                }
            }

            foreach (var newItem in allItems)
            {
                Repository.Insert(newItem);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public double GetUSDRate(string from) => GetCurrencyRate(from, "USD");
    }
}
