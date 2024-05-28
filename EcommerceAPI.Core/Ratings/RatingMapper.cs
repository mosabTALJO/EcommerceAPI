using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.Ratings
{
    public class RatingMapper : Profile
    {
        public RatingMapper()
        {
            CreateMap<Rating, RatingVM>().ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Order.CustomerId))
                                         .ForMember(dest => dest.RatingType, opt => opt.MapFrom(src => src.RatingType.ToString()));
        }
    }
}
