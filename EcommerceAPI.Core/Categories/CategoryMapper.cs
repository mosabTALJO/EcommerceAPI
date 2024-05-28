using AutoMapper;
using EcommerceAPI.Core.Categories.Commands;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.Categories
{
    public class CategoryMapper : Profile
    {
        public CategoryMapper()
        {
            CreateMap<Category, CategoryVM>()
                .ForMember(dst => dst.FullName, opt => opt.MapFrom(src => src.HelperName))
                .ReverseMap();
            CreateMap<Category, CreateCategoryCommand>().ReverseMap();
        }
    }
}