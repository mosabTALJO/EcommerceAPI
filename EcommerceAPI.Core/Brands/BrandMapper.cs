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
            CreateMap<Brand, BrandVM>().ForMember(dst => dst.ProductsCount, ex => ex.Ignore());
            CreateMap<BrandVM, Brand>();
            CreateMap<Brand, CreateBrandCommand>().ReverseMap();
            CreateMap<Brand, UpdateBrandCommand>().ReverseMap();
        }
    }
}
