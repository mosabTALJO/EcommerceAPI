using AutoMapper;
using EcommerceAPI.Core.Coupons.Commands;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.ViewModels;

namespace EcommerceAPI.Core.Coupons
{
    public class CouponMapper : Profile
    {
        public CouponMapper()
        {
            CreateMap<Coupon, CouponLiteVM>().ReverseMap();
            CreateMap<Coupon, CouponVM>().ReverseMap();
            CreateMap<Coupon, CreateCouponCommand>().ReverseMap();
        }
    }
}
