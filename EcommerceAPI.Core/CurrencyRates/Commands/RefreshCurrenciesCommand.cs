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
            private readonly IMemoryCache _memoryCache;
            private readonly IUnitOfWork _unitOfWork;
            public RefreshCurrenciesCommandHandler(ICurrencyRateService currencyRateService, IUnitOfWork unitOfWork, IMemoryCache memoryCache)
            {
                _currencyRateService = currencyRateService;
                _unitOfWork = unitOfWork;
                _memoryCache = memoryCache;
            }
            public async Task<RequestResult<bool>> Handle(RefreshCurrenciesCommand request, CancellationToken cancellationToken)
            {
                if (await CurrencyScrapper.Scrap())
                {
                    _currencyRateService.ClearCache();
                    return new RequestResult<bool>(true);
                }
                return new RequestResult<bool>(ActionResultType.BadRequest, "scraper ERROR");
            }
        }
    }
}
