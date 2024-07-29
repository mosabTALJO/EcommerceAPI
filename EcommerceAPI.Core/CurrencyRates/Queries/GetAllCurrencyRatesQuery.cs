using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.CurrencyRates.Queries
{
    public class GetAllCurrencyRatesQuery : IRequest<RequestResult<CurrencyRateVM[]>>
    {
        public class GetAllCurrencyRatesQueryHandler : IRequestHandler<GetAllCurrencyRatesQuery, RequestResult<CurrencyRateVM[]>>
        {
            private readonly ICurrencyRateService _currencyRateService;
            private readonly IMapper _mapper;

            public GetAllCurrencyRatesQueryHandler(ICurrencyRateService currencyRateService, IMapper mapper)
            {
                _currencyRateService = currencyRateService;
                _mapper = mapper;
            }

            public async Task<RequestResult<CurrencyRateVM[]>> Handle(GetAllCurrencyRatesQuery request, CancellationToken cancellationToken)
            {
                return new RequestResult<CurrencyRateVM[]>(await _currencyRateService.Queryable().To<CurrencyRateVM>(_mapper).ToArrayAsync());
            }
        }
    }
}
