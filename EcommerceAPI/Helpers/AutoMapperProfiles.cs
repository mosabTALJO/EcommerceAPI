using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Web.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Collection, CollectionVM>().ReverseMap();
            CreateMap<CollectionItem, CollectionItemVM>().ForMember(dst => dst.Product, src => src.Ignore());
            CreateMap<CollectionItemVM, CollectionItem>().ForMember(dst => dst.Product, src => src.Ignore());

            CreateMap<CurrencyRate, CurrencyRateVM>().ReverseMap();
            CreateMap<CurrencyRate, CurrencyRateLiteVM>().ReverseMap();
        }
    }
}
