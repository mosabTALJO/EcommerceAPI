using AutoMapper;
using EcommerceAPI.Core.BaseItems.Command;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.BaseItems
{
    public class BaseItemMapper : Profile
    {
        public BaseItemMapper()
        {
            CreateMap<BaseItem, BaseItemVM>().ReverseMap();
            CreateMap<BaseItem, CreateBaseItemCommand>().ReverseMap();
        }
    }
}
