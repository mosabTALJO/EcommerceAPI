using AutoMapper;
using EcommerceAPI.Core.Notifications.Commands;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.Notifications
{
    public class NotificationMapper : Profile
    {
        public NotificationMapper()
        {
            CreateMap<Notification, NotificationVM>().ReverseMap();
            CreateMap<Notification, CreateNotificationCommand>().ReverseMap();
            CreateMap<Notification, SendNotificationCommand>().ReverseMap();
        }
    }
}
