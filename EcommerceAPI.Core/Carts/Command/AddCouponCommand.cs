using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Carts.Command
{
    public class AddCouponCommand : IRequest<RequestResult<bool>>
    {
        public string CouponCode { get; set; }
        public string UserId { get; set; }
        public class AddCouponCommandHandler : IRequestHandler<AddCouponCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IProductVariantService _productVariantService;
            private readonly ICouponService _couponService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly UserManager<AppUser> _userManager;

            public AddCouponCommandHandler(IUnitOfWork unitOfWork, ICouponService couponService, IProductVariantService productVariantService, IOrderService orderService, UserManager<AppUser> userManager)
            {
                _unitOfWork = unitOfWork;
                _couponService = couponService;
                _productVariantService = productVariantService;
                _orderService = orderService;
                _userManager = userManager;
            }

            public async Task<RequestResult<bool>> Handle(AddCouponCommand command, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(command.CouponCode))
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Coupon code is empty");

                var order = _orderService.Queryable().Include(x => x.OrderItems)
                .FirstOrDefault(x => x.OrderStatus == OrderStatus.CartOrder && x.CustomerId == command.UserId);
                if (order?.OrderItems?.Any() != true)
                    return new RequestResult<bool>(ActionResultType.NotFound, "No items in the cart");

                var coupon = _couponService.CheckCoupon(command.CouponCode, out string error);
                if (coupon is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "orders.coupon-not-found");

                bool couponExists = false;
                foreach (var oi in order.OrderItems)
                {
                    var pv = await _productVariantService.Queryable().Where(x => x.Id == oi.ProductVariantId)
                        .Select(x => new { x.Id, x.ProductId, x.Product!.BrandId, x.Product.CategoryId }).FirstOrDefaultAsync();
                    if (pv is null)
                        continue;

                    if (coupon.Type == CouponType.General
                    || coupon.Type == CouponType.Product && pv.ProductId == coupon.TypeId
                    || coupon.Type == CouponType.Brand && pv.BrandId == coupon.TypeId
                    || coupon.Type == CouponType.Category && pv.CategoryId == coupon.TypeId)
                    {
                        oi.CoupunDiscount = coupon.Rate;
                        couponExists = true;
                    }
                }

                if (!couponExists)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "The Coupon is not applicable for the selected items");

                order.CoupunDiscountRate = coupon.Rate;
                order.DiscountCoupunId = coupon.Id;
                _orderService.UpdateOrder(order);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
