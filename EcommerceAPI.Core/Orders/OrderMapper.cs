using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.Orders
{
    public class OrderMapper : Profile
    {
        public OrderMapper()
        {
            CreateMap<Order, OrderVM>()
                .ForMember(dst => dst.CustomerName,
                    opt => opt.MapFrom(src => src.Customer.DisplayName)).ReverseMap();
            CreateMap<OrderItem, OrderItemVM>().ReverseMap();
                //.ForMember(dst => dst.HasLogs,
                //opt => opt.MapFrom(src => src.Logs.Any())).ReverseMap();

            CreateMap<Order, OrderLiteVM>()
                .ForMember(dst => dst.CustomerName, opt => opt.MapFrom(src => src.Customer.DisplayName));

            CreateMap<Order, NewOrderVM>().ReverseMap();
            CreateMap<OrderItem, NewOrderItemVM>().ReverseMap();

            CreateMap<OrderPayment, OrderPaymentVM>().ReverseMap();
        }
    }
}
