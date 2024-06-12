using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.Users
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<AppUser, UserVM>().ReverseMap();
        }
    }
}
