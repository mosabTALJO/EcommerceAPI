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
            private readonly IProductService _productService;
            private readonly ICouponService _couponService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly UserManager<AppUser> _userManager;

            public AddCouponCommandHandler(IUnitOfWork unitOfWork, ICouponService couponService, IProductService productService, IOrderService orderService, UserManager<AppUser> userManager)
            {
                _unitOfWork = unitOfWork;
                _couponService = couponService;
                _productService = productService;
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

                if (coupon.IsForCustomerFirstOrder)
                {
                    if (_orderService.Queryable().Any(o => o.CustomerId == order.CustomerId && o.OrderStatus > OrderStatus.Pending))
                        return new RequestResult<bool>(ActionResultType.BadRequest, "The Coupon is not applicable for the current user");
                }

                bool couponExists = false;
                foreach (var oi in order.OrderItems)
                {
                    var p = _productService.Queryable().Include(x => x.ProductVariants).AsNoTracking().FirstOrDefault(x => x.Id == oi.ProductId);
                    if (p is null)
                        continue;
                    if (!p.ProductVariants.Any(x => x.Id == oi.ProductVariantId))
                        continue;

                    if (coupon.Type == CouponType.General
                    || coupon.Type == CouponType.Product && p.Id == coupon.TypeId
                    || coupon.Type == CouponType.Brand && p.BrandId == coupon.TypeId
                    || coupon.Type == CouponType.Category && p.CategoryId == coupon.TypeId)
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
