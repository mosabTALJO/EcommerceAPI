using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.Extensions.Caching.Memory;

namespace EcommerceAPI.Services
{
    public interface ICurrencyRateService : IService<CurrencyRate>
    {
        double GetCurrencyRate(string currency);
        void ClearCache();
    }

    public class CurrencyRateService : Service<CurrencyRate>, ICurrencyRateService
    {
        readonly IMemoryCache _cache;
        public CurrencyRateService(IRepository<CurrencyRate> repo, IMemoryCache cache) : base(repo)
        {
            _cache = cache;
        }

        public double GetCurrencyRate(string currency)
        {
            currency ??= "usd";
            var key = "CurrencyChange_" + currency.ToLower();
            if (!_cache.TryGetValue(key, out double rateValue))
            {
                var currencyType = GetTypeFromCurrency(currency.ToLower());
                var rateRecord = Repository.Queryable().FirstOrDefault(x => x.Type == currencyType);
                rateValue = rateRecord == null ? 0 : rateRecord.Rate * rateRecord.FeeRate;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(1));

                _cache.Set(key, rateValue, cacheEntryOptions);
            }

            return rateValue;
        }

        private static CurrencyRateTypes GetTypeFromCurrency(string currency) =>
                currency == "usd" ? CurrencyRateTypes.USD :
                currency == "kwd" ? CurrencyRateTypes.KWD :
                currency == "sar" ? CurrencyRateTypes.SAR :
                currency == "aed" ? CurrencyRateTypes.AED :
                currency == "omr" ? CurrencyRateTypes.OMR :
                currency == "qar" ? CurrencyRateTypes.QAR :
                currency == "egp" ? CurrencyRateTypes.EGP :
                currency == "iqd" ? CurrencyRateTypes.IQD :
                currency == "mad" ? CurrencyRateTypes.MAD :
                currency == "jod" ? CurrencyRateTypes.JOD :
                currency == "dzd" ? CurrencyRateTypes.DZD :
                currency == "bhd" ? CurrencyRateTypes.BHD : CurrencyRateTypes.KWD;

        public void ClearCache()
        {
            _cache.Remove("CurrencyChange_usd");
            _cache.Remove("CurrencyChange_kwd");
            _cache.Remove("CurrencyChange_sar");
            _cache.Remove("CurrencyChange_aed");
            _cache.Remove("CurrencyChange_omr");
            _cache.Remove("CurrencyChange_qar");
            _cache.Remove("CurrencyChange_egp");
            _cache.Remove("CurrencyChange_iqd");
            _cache.Remove("CurrencyChange_mad");
            _cache.Remove("CurrencyChange_jod");
            _cache.Remove("CurrencyChange_dzd");
            _cache.Remove("CurrencyChange_bhd");
        }
    }
}
