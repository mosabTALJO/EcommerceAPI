using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EcommerceAPI.Core.Carts.Command
{
    public class OnDoorPaymentPendingOrderCommand : IRequest<RequestResult<bool>>
    {
        public int UserAddressId { get; set; }
        public string? UserId { get; set; }
        public string? Currency { get; set; }
        public OrderDeviceType? OrderDeviceType { get; set; }
        public class OnDoorPaymentPendingOrderCommandHandler : IRequestHandler<OnDoorPaymentPendingOrderCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly UserManager<AppUser> _userManager;
            private readonly ICouponService _couponService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICountryService _countryService;
            private readonly IProductVariantService _productVariantService;
            private readonly ICollectionService _collectionService;
            private readonly ICurrencyRateService _currencyRateService;

            public OnDoorPaymentPendingOrderCommandHandler(IOrderService orderService, UserManager<AppUser> userManager, ICouponService couponService, IUnitOfWork unitOfWork, ICountryService countryService, IProductVariantService productVariantService, ICollectionService collectionService, IOrderLogService orderLogService, ICurrencyRateService currencyRateService)
            {
                _orderService = orderService;
                _userManager = userManager;
                _couponService = couponService;
                _unitOfWork = unitOfWork;
                _countryService = countryService;
                _productVariantService = productVariantService;
                _collectionService = collectionService;
                _orderLogService = orderLogService;
                _currencyRateService = currencyRateService;
            }
            public async Task<RequestResult<bool>> Handle(OnDoorPaymentPendingOrderCommand command, CancellationToken cancellationToken)
            {
                var cartOrder = _orderService.Queryable().Include(x => x.OrderItems).AsNoTracking()
              .FirstOrDefault(x => x.OrderStatus == OrderStatus.CartOrder && x.CustomerId == command.UserId);
                if (cartOrder?.OrderItems?.Any() != true)
                    return new RequestResult<bool>(ActionResultType.NotFound, "No items in the cart");

                var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";

                var u = _userManager.Users.Include(x => x.UserAddresses).FirstOrDefault(x => x.Id == command.UserId);
                if (u is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "User not found");

                var o = await _orderService.GetOrderDetails(cartOrder.Id);
                if (o.OrderItems.Any(x => x.ProductVariant?.Product?.OnlinePaymentOnly == true))
                    return new RequestResult<bool>(ActionResultType.BadRequest, $"The item {o.OrderItems.First(x => x.ProductVariant?.Product?.OnlinePaymentOnly == true).Id} can't be paid on door");

                var address = u.UserAddresses?.FirstOrDefault(x => x.Id == command.UserAddressId);
                if (address is null)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Address not found");
                if (string.IsNullOrEmpty(address.PhoneNumber))
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Phone number in address is required");
                if (!CartOrderOperations.IsPhoneNumber(address.PhoneNumber))
                    return new RequestResult<bool>(ActionResultType.BadRequest, lang == "en" ? "Phone number is incorrect" : "رقم الهاتف غير صحيح");

                var c = await _countryService.Queryable().AsNoTracking().Include(x => x.Cities).FirstOrDefaultAsync(x => x.Code.ToLower() == address.Country.ToLower()) ??
                await _countryService.Queryable().AsNoTracking().Include(x => x.Cities).FirstOrDefaultAsync(x => x.Code.ToLower() == "kwd");

                var city = c.Cities.FirstOrDefault(x => x.Code == address.City);

                Coupon coupon = null;
                if (o.DiscountCoupunId.HasValue)
                {
                    coupon = _couponService.CheckCoupon(o.DiscountCoupunId.Value, out string error);
                    if (coupon is null)
                        return new RequestResult<bool>(ActionResultType.BadRequest, "Coupun Error: " + error);
                }
                o.OrderStatus = OrderStatus.OnDoorPaymentPending;
                o.PaymentType = OrderPaymentType.OnDoor;
                o.OrderDeviceType = command.OrderDeviceType;

                bool areProductsValid = await CartOrderOperations.CalcualteAndValidateOrderItems(_countryService, _productVariantService, _couponService, _collectionService, _currencyRateService, o, address, coupon, command.Currency);
                _orderService.UpdateOrder(o, true);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    await _orderLogService.OrderOnDoorPaymentPendingLog(o.Id, command.UserId);
                    foreach (var item in o.OrderItems)
                        await _orderLogService.ItemCreatedLog(item.Id, o, command.UserId);

                    if (areProductsValid)
                        return new RequestResult<bool>(true);

                    return new RequestResult<bool>(ActionResultType.StatusCode634, lang == "en" ? "Some of products is out of stock, your cart has been updated" : "بعض المنتجات نفذت من المستودع، تم تحديث السلة الخاصة بك");
                }

                return new RequestResult<bool>(ActionResultType.BadRequest, lang == "en" ? "An error occured while saving" : "حدث خطأ أثناء الحفظ");
            }
        }
    }
}
