using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.CurrencyRates
{
    public class CurrencyRateMapper : Profile
    {
        public CurrencyRateMapper()
        {
            CreateMap<CurrencyRate, CurrencyRateVM>()
                .ReverseMap();
            CreateMap<CurrencyRate, CurrencyRateLiteVM>().ReverseMap();
        }
    }
}
