using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public interface ICouponService : IService<Coupon>
    {
        Coupon CheckCoupon(string code, out string error);
        Coupon CheckCoupon(int CouponId, out string error);
        Task<Coupon> CreateCustomerCoupon(double? rate, string customerId);
    }

    public class CouponService : Service<Coupon>, ICouponService
    {
        public CouponService(IRepository<Coupon> repo) : base(repo)
        {
        }

        public Coupon CheckCoupon(string code, out string error)
        {
            error = "";
            var c = Repository.Queryable().AsNoTracking().FirstOrDefault(x => !x.DeletedAt.HasValue && x.Code == code);
            if (c is null)
                error = "NotFound";
            else if (c.RemainingCount <= 0)
                error = "Coupon is not active";
            else if (c.ExpireDate.ToUniversalTime() < DateTime.UtcNow)
                error = "Expired Coupon";

            return string.IsNullOrEmpty(error) ? c : null;
        }

        public Coupon CheckCoupon(int CouponId, out string error)
        {
            error = "";
            var c = Repository.Queryable().AsNoTracking().FirstOrDefault(x => !x.DeletedAt.HasValue && x.Id == CouponId);
            if (c is null)
                error = "NotFound";
            else if (c.RemainingCount <= 0)
                error = "Coupon is not active";
            else if (c.ExpireDate.ToUniversalTime() < DateTime.UtcNow)
                error = "Expired Coupon";

            return string.IsNullOrEmpty(error) ? c : null;
        }

        public async Task<Coupon> CreateCustomerCoupon(double? rate, string customerId)
        {
            var coupon = new Coupon
            {
                Rate = rate ?? 0,
                CustomerId = customerId,
                Code = $"CustomerCoupon:{customerId}",
                NameAr = "",
                NameEn = "",
                NameTr = "",
                ExpireDate = DateTime.Now.AddDays(7),
                Count = 1,
                RemainingCount = 1,
                IsActive = true,
                Type = CouponType.General
            };

            return coupon;
        }
    }
}
