using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Scrapper;

namespace EcommerceAPI.Core.CurrencyRates.Commands
{
    public class RefreshCurrenciesCommand : IRequest<RequestResult<bool>>
    {
        public class RefreshCurrenciesCommandHandler : IRequestHandler<RefreshCurrenciesCommand, RequestResult<bool>>
        {
            private readonly ICurrencyRateService _currencyRateService;
            private readonly ICountryService _countryService;
            private readonly IMemoryCache _memoryCache;
            private readonly IUnitOfWork _unitOfWork;
            public RefreshCurrenciesCommandHandler(ICurrencyRateService currencyRateService, IUnitOfWork unitOfWork, IMemoryCache memoryCache, ICountryService countryService = null)
            {
                _currencyRateService = currencyRateService;
                _unitOfWork = unitOfWork;
                _memoryCache = memoryCache;
                _countryService = countryService;
            }
            public async Task<RequestResult<bool>> Handle(RefreshCurrenciesCommand request, CancellationToken cancellationToken)
            {
                List<CurrencyRate> allItems = await CurrencyScrapper.Scrap(ICurrencyRateService.CurrencyCodes);

                var dbItems = _currencyRateService.Queryable().ToArray();
                foreach (var dbItem in dbItems)
                {
                    var newItem = allItems.FirstOrDefault(x => x.FromCurrencyCode == dbItem.FromCurrencyCode && x.ToCurrencyCode == dbItem.ToCurrencyCode);
                    if (newItem != null)
                    {
                        dbItem.Rate = newItem.Rate;
                        _currencyRateService.Update(dbItem);
                        allItems.Remove(newItem);
                    }
                }

                foreach (var newItem in allItems)
                {
                    _currencyRateService.Insert(newItem);
                }

                await _unitOfWork.SaveChangesAsync();

                await _currencyRateService.ClearCache();
                return new RequestResult<bool>(true);
            }
        }
    }
}
