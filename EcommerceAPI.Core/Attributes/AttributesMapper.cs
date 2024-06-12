using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.Attributes
{
    public class AttributesMapper : Profile
    {
        public AttributesMapper()
        {
            CreateMap<ProductAttribute, ProductAttributeVM>().ReverseMap();
            CreateMap<ProductVariantAttribute, ProductVariantAttributeVM>().ForMember(dst => dst.AttributeName, ex => ex.MapFrom(src => src.ProductAttribute!.Name));
            CreateMap<ProductVariantAttributeVM, ProductVariantAttribute>();
        }
    }
}
