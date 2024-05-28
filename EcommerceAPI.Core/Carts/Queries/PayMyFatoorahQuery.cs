using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EcommerceAPI.Core.Carts.Queries
{
    public class PayMyFatoorahQuery : IRequest<RequestResult<string>>
    {
        public int UserAddressId { get; set; }
        public string? UserId { get; set; }
        public string? Currency { get; set; }
        public string? IdCard { get; set; }
        public ProductNotFound? ProductNotFound { get; set; } = null;
        public OrderDeviceType? OrderDeviceType { get; set; } = null;
        public class PayMyFatoorahQueryHandler : IRequestHandler<PayMyFatoorahQuery, RequestResult<string>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly IOrderPaymentService _orderPaymentService;
            private readonly UserManager<AppUser> _userManager;
            private readonly ICouponService _couponService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICountryService _countryService;
            private readonly IProductService _productService;
            private readonly ICollectionService _collectionService;
            private readonly ICurrencyRateService _currencyRateService;

            public PayMyFatoorahQueryHandler(IOrderService orderService, UserManager<AppUser> userManager, ICouponService couponService, IUnitOfWork unitOfWork, ICountryService countryService, IProductService productService, ICollectionService collectionService, IOrderLogService orderLogService, ICurrencyRateService currencyRateService, IOrderPaymentService orderPaymentLogService)
            {
                _orderService = orderService;
                _userManager = userManager;
                _couponService = couponService;
                _unitOfWork = unitOfWork;
                _countryService = countryService;
                _productService = productService;
                _collectionService = collectionService;
                _orderLogService = orderLogService;
                _currencyRateService = currencyRateService;
                _orderPaymentService = orderPaymentLogService;
            }

            public async Task<RequestResult<string>> Handle(PayMyFatoorahQuery query, CancellationToken cancellationToken)
            {
                var cartOrder = _orderService.Queryable().Include(x => x.OrderItems).AsNoTracking()
                    .FirstOrDefault(x => x.OrderStatus == OrderStatus.CartOrder && x.CustomerId == query.UserId);
                if (cartOrder?.OrderItems?.Any() != true)
                    return new RequestResult<string>(ActionResultType.NotFound, "No items in the cart");

                var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";
                var u = _userManager.Users.Include(x => x.UserAddresses).FirstOrDefault(x => x.Id == query.UserId);
                if (u is null)
                    return new RequestResult<string>(ActionResultType.NotFound, "User not found");
                if (!string.IsNullOrEmpty(query.IdCard))
                {
                    u.IdCardImage = query.IdCard;
                    var userResult = await _userManager.UpdateAsync(u);
                    if (!userResult.Succeeded)
                        return new RequestResult<string>(ActionResultType.BadRequest, "error while savind id card");
                }

                var o = await _orderService.GetOrderDetails(cartOrder.Id);
                var address = u.UserAddresses?.FirstOrDefault(x => x.Id == query.UserAddressId);
                if (address is null)
                    return new RequestResult<string>(ActionResultType.BadRequest, "Address not found");
                if (string.IsNullOrEmpty(address.PhoneNumber))
                    return new RequestResult<string>(ActionResultType.BadRequest, "Phone number in address is required");
                if (!CartOrderOperations.IsPhoneNumber(address.PhoneNumber))
                    return new RequestResult<string>(ActionResultType.BadRequest, lang == "en" ? "Phone number is incorrect" : "رقم الهاتف غير صحيح");

                var c = await _countryService.Queryable().AsNoTracking().Include(x => x.Cities).FirstOrDefaultAsync(x => x.Code.ToLower() == address.Country.ToLower()) ??
                await _countryService.Queryable().AsNoTracking().Include(x => x.Cities).FirstOrDefaultAsync(x => x.Code.ToLower() == "kwd");

                Coupon coupon = null;
                if (o.DiscountCoupunId.HasValue)
                {
                    coupon = _couponService.CheckCoupon(o.DiscountCoupunId.Value, out string error);
                    if (coupon is null)
                        return new RequestResult<string>(ActionResultType.BadRequest, "Coupun Error: " + error);
                }
                o.PaymentType = OrderPaymentType.Online;
                o.ProductNotFound = query.ProductNotFound;
                o.OrderDeviceType = query.OrderDeviceType;

                bool areProductsValid = await CartOrderOperations.CalcualteAndValidateOrderItems(_countryService, _productService, _couponService, _collectionService, _currencyRateService, o, address, coupon, query.Currency);

                _orderService.UpdateOrder(o, true);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    await _orderLogService.OrderCreatedLog(o.Id, query.UserId);
                    foreach (var item in o.OrderItems)
                        await _orderLogService.ItemCreatedLog(item.Id, o, query.UserId);

                    if (areProductsValid)
                    {
                        string myFatoorahError = "";
                        try
                        {
                            var kwdCurrencyRate = _currencyRateService.GetCurrencyRate("kwd");
                            var totalKwdPrice = o.TotalTLPrice * kwdCurrencyRate;

                            var sendPaymentRequest = CartOrderOperations.GeneratePaymentRequest(o, u, address, totalKwdPrice);
                            var x = await MyFatoorahHelper.SendPayment(sendPaymentRequest);
                            if (x?.IsSuccess == true)
                            {
                                _orderPaymentService.Insert(new OrderPayment
                                {
                                    OrderId = o.Id,
                                    Amount = totalKwdPrice,
                                    PaymentId = "",
                                    InvoiceId = x.Data?.InvoiceId ?? 0,
                                    InvoiceStatus = "pending",//x.Data.InvoiceStatus
                                    InvoiceURL = x.Data?.InvoiceURL,
                                });
                                return new RequestResult<string>(x.Data.InvoiceURL);
                            }

                            myFatoorahError = x?.Message;
                        }
                        catch (Exception ex)
                        {
                            myFatoorahError = ex.Message + ", " + ex.InnerException?.Message;
                        }

                        //change it back to cart
                        o.OrderStatus = OrderStatus.CartOrder;
                        _orderService.UpdateOrder(o, true);
                        await _unitOfWork.SaveChangesAsync();
                        return new RequestResult<string>(ActionResultType.BadRequest, "An error occured in payment\nerror: " + myFatoorahError);
                    }

                    return new RequestResult<string>(ActionResultType.StatusCode634, lang == "en" ? "Some of products is out of stock, your cart has been updated" : "بعض المنتجات نفذت من المستودع، تم تحديث السلة الخاصة بك");
                }

                return new RequestResult<string>(ActionResultType.BadRequest, lang == "en" ? "An error occured while saving" : "حدث خطأ أثناء الحفظ");
            }
        }
    }
}
