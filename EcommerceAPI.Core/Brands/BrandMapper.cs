using AutoMapper;
using EcommerceAPI.Core.Brands.Commands;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.Brands
{
    public class BrandMapper : Profile
    {
        public BrandMapper()
        {
            CreateMap<Brand, BrandVM>().ReverseMap();
            CreateMap<Brand, CreateBrandCommand>().ReverseMap();
            CreateMap<Brand, UpdateBrandCommand>().ReverseMap();
        }
    }
}
