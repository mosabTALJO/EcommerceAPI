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
                .ForMember(dst => dst.LastUpdatedDate, opt => opt.MapFrom(src => src.LastUpdatedDate.Value.ToLocalTime()))
                .ReverseMap();
            CreateMap<CurrencyRate, CurrencyRateLiteVM>().ReverseMap();
        }
    }
}
