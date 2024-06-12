using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.OrderLogs
{
    public class OrderLogMapper : Profile
    {
        public OrderLogMapper()
        {
            CreateMap<OrderLog, OrderLogVM>().ReverseMap();
        }
    }
}
