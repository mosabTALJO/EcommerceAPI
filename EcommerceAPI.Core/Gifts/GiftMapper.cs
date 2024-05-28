using AutoMapper;
using EcommerceAPI.Core.Gifts.Commands;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.Gifts
{
    public class GiftMapper : Profile
    {
        public GiftMapper()
        {
            CreateMap<GiftPriceRange, GiftPriceRangeVM>().ReverseMap();
            CreateMap<GiftPriceRange, CreateGiftPriceRangeCommand>().ReverseMap();
            CreateMap<Gift, GiftVM>().ReverseMap();
            CreateMap<Gift, GiftLiteVM>().ForMember(dst => dst.GiftType, opt => opt.MapFrom(src => src.GiftType.ToString()));
            CreateMap<Gift, GiftPostVM>().ReverseMap();
        }
    }
}
