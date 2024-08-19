using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using FirebaseAdmin.Messaging;
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
                

                await _currencyRateService.ClearCache();
                return new RequestResult<bool>(true);
            }
        }
    }
}
