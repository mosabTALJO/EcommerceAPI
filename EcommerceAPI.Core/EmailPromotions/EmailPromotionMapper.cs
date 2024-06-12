using AutoMapper;
using EcommerceAPI.Core.EmailPromotions.Commands;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.EmailPromotions
{
    public class EmailPromotionMapper : Profile
    {
        public EmailPromotionMapper()
        {
            CreateMap<EmailPromotion, EmailPromotionVM>().ReverseMap();
            CreateMap<EmailPromotion, EmailPromotionLiteVM>().ReverseMap();

            CreateMap<EmailPromotion, CreateEmailPromotionCommand>().ReverseMap();
        }
    }
}
