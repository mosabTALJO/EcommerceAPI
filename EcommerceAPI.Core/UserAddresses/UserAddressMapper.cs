using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.UserAddresses
{
    public class UserAddressMapper : Profile
    {
        public UserAddressMapper()
        {
            CreateMap<UserAddress, UserAddressVM>()
                .ForMember(x => x.City, dst => dst.MapFrom(x => x.City.NameEn))
                .ForMember(x => x.Country, dst => dst.MapFrom(x => x.Country.NameEn))
                .ReverseMap();
        }
    }
}
