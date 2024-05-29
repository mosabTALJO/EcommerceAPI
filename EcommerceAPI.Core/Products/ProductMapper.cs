using AutoMapper;
using EcommerceAPI.Core.Products.Commands;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.Products
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<Product, ProductLiteVM>()
                .ForMember(dst => dst.MainImageRow, opt => opt.MapFrom(src => src.MainImage))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.NameEn))
                .ForMember(dst => dst.BrandName, opt => opt.MapFrom(src => src.Brand.NameEn));

            CreateMap<ProductVariant, ProductVariantVM>().ReverseMap();

            CreateMap<Product, ProductVM>()
                .ForMember(dst => dst.Images,
                    opt => opt.MapFrom(src => src.Images == null ? new List<string>() : src.Images.Split(",", StringSplitOptions.TrimEntries).ToList()));

            CreateMap<Product, CreateProductCommand>()
                .ForMember(dst => dst.Images,
                    opt => opt.MapFrom(src => src.Images == null ? new List<string>() : src.Images.Split(",", StringSplitOptions.TrimEntries).ToList()));

            CreateMap<ProductVM, Product>()
                .ForMember(dst => dst.Images,
                    opt => opt.MapFrom(src => string.Join(",", src.Images)));

            CreateMap<CreateProductCommand, Product>()
                .ForMember(dst => dst.Images,
                    opt => opt.MapFrom(src => string.Join(",", src.Images)));
        }
    }
}
