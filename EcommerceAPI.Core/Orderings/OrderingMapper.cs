using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.Orderings
{
    public class OrderingMapper : Profile
    {
        public OrderingMapper()
        {
            CreateMap<OrderCategoryBrand, OrderCategoryBrandVM>().ReverseMap();
        }
    }
}
