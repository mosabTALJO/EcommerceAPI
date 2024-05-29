using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EcommerceAPI.Core.Carts.Command
{
    public class NoAccountPurchaseCommand : IRequest<RequestResult<NoAccountPaymentVM>>
    {
        public CartItemVM[] Items { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string FullAddress { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Currency { get; set; }
        public string? CouponCode { get; set; }
        public string? IdCard { get; set; }
        public OrderPaymentType OrderPaymentType { get; set; }
        public ProductNotFound? ProductNotFound { get; set; } = null;
        public OrderDeviceType? OrderDeviceType { get; set; }
        public class NoAccountPurchaseCommandHandler : IRequestHandler<NoAccountPurchaseCommand, RequestResult<NoAccountPaymentVM>>
        {
            private readonly IOrderService _orderService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly UserManager<AppUser> _userManager;
            private readonly ICurrencyRateService _currencyRateService;
            private readonly IProductService _productService;
            private readonly ICouponService _couponService;
            private readonly ICountryService _countryService;
            private readonly ICollectionService _collectionService;
            private readonly IOrderLogService _orderLogService;
            private readonly IOrderPaymentService _orderPaymentService;
            //private readonly IEmailSender _emailSender;
            public NoAccountPurchaseCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ICurrencyRateService currencyRateService, IProductService productService, ICouponService couponService, ICountryService countryService, ICollectionService collectionService, IOrderLogService orderLogService, IOrderPaymentService orderPaymentService/*, IEmailSender emailSender*/)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _userManager = userManager;
                _currencyRateService = currencyRateService;
                _productService = productService;
                _couponService = couponService;
                _countryService = countryService;
                _collectionService = collectionService;
                _orderLogService = orderLogService;
                _orderPaymentService = orderPaymentService;
                //_emailSender = emailSender;
            }
            public async Task<RequestResult<NoAccountPaymentVM>> Handle(NoAccountPurchaseCommand command, CancellationToken cancellationToken)
            {
                if (command.Items?.Any() != true)
                    return new RequestResult<NoAccountPaymentVM>(ActionResultType.BadRequest, "No items in cart");

                if (string.IsNullOrEmpty(command?.FirstName) || string.IsNullOrEmpty(command?.LastName) || string.IsNullOrEmpty(command?.Email)
                    || string.IsNullOrEmpty(command.PhoneNumber) || string.IsNullOrEmpty(command.FullAddress) || string.IsNullOrEmpty(command.Country)
                    || string.IsNullOrEmpty(command.City))
                    return new RequestResult<NoAccountPaymentVM>(ActionResultType.BadRequest, "Please fill required fields required");

                var user = await _userManager.FindByEmailAsync(command.Email + "_NotRegistered");
                if (user == null)
                {
                    user = new AppUser
                    {
                        UserName = command.Email + "_NotRegistered",
                        Email = command.Email + "_NotRegistered",
                        PhoneNumber = command.PhoneNumber,
                        DisplayName = command.FirstName + " " + command.LastName,
                        //IsActive = true,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        CreatedAt = DateTime.Now,
                        UserType = UserTypes.NotRegisteredCustomer,
                        Country = command.Country,
                        IdCardImage = command.IdCard != null ? command.IdCard : null,
                    };
                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                        return new RequestResult<NoAccountPaymentVM>(ActionResultType.BadRequest, "ERROR in entered information");
                }

                var address = new UserAddress
                {
                    AppUserId = user.Id,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    PhoneNumber = command.PhoneNumber,
                    Country = command.Country,
                    City = command.City,
                    FullAddress = command.FullAddress,
                };
                //_userAddressService.Insert(address);
                //await _unitOfWork.SaveChangesAsync();
                var c = await _countryService.Queryable().AsNoTracking().Include(x => x.Cities).FirstOrDefaultAsync(x => x.Code.ToLower() == address.Country.ToLower()) ??
                await _countryService.Queryable().AsNoTracking().Include(x => x.Cities).FirstOrDefaultAsync(x => x.Code.ToLower() == "kwd");

                //if (c.IsRequestIdCard == true && string.IsNullOrEmpty(command.IdCard))
                //    return new RequestResult<NoAccountPaymentVM>(ActionResultType.BadRequest, "you have to upload id card");

                var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";

                var o = new Order()
                {
                    OrderStatus = OrderStatus.CartOrder,
                    CustomerId = user.Id,
                    OrderItems = new List<OrderItem>(),
                    Currency = command.Currency,
                    Country = command.Country,
                    CurrencyRate = _currencyRateService.GetCurrencyRate(command.Currency),
                    ProductNotFound = command.ProductNotFound,
                };

                Coupon coupon = null;
                if (!string.IsNullOrEmpty(command.CouponCode))
                {
                    coupon = _couponService.CheckCoupon(command.CouponCode, out string couponError);
                    if (coupon is null)
                        return new RequestResult<NoAccountPaymentVM>(ActionResultType.NotFound, "orders.coupon-not-found");

                    if (coupon.IsForCustomerFirstOrder)
                    {
                        if (_userManager.FindByEmailAsync(user.Email).Result?.Orders?.Any() == true)
                            return new RequestResult<NoAccountPaymentVM>(ActionResultType.BadRequest, "The Coupon is not applicable for the current user");
                    }
                }
                string error = "";
                bool couponExists = false;
                foreach (var item in command.Items)
                {
                    var oi = new OrderItem() { ProductId = item.ProductId, ProductVariantId = item.ProductVariantId, Qty = item.Quantity, OrderItemState = OrderItemState.Pending };
                    o.OrderItems.Add(oi);

                    var p = _productService.Queryable().Include(x => x.ProductVariants).AsNoTracking().FirstOrDefault(x => x.Id == item.ProductId);
                    bool productExists = p != null && p.ProductVariants.Any(x => x.Id == item.ProductVariantId);

                    if (!productExists)
                    {
                        return new RequestResult<NoAccountPaymentVM>(ActionResultType.NotFound, $"item {oi.Id} is not found");
                        //if (oi != null)
                        //    oi.Qty = 0;
                        //error = "ProductId not found : " + item.ProductId + "\n";
                    }
                    else
                    {
                        if (coupon == null)
                            continue;
                        if (coupon.Type == CouponType.General
                        || (coupon.Type == CouponType.Product && p.Id == coupon.TypeId)
                        || (coupon.Type == CouponType.Brand && p.BrandId == coupon.TypeId)
                        || (coupon.Type == CouponType.Category && p.CategoryId == coupon.TypeId))
                        {
                            oi.CoupunDiscount = coupon.Rate;
                            couponExists = true;
                        }
                    }
                }
                if (coupon != null && !couponExists)
                    return new RequestResult<NoAccountPaymentVM>(ActionResultType.BadRequest, "The Coupon is not applicable for the selected items");

                if (coupon != null)
                {
                    o.CoupunDiscountRate = coupon.Rate;
                    o.DiscountCoupunId = coupon.Id;
                }

                o.PaymentType = command.OrderPaymentType;
                o.OrderStatus = command.OrderPaymentType == OrderPaymentType.OnDoor ? OrderStatus.OnDoorPaymentPending : OrderStatus.Pending;
                o.ProductNotFound = command.ProductNotFound;
                o.OrderDeviceType = command.OrderDeviceType;

                bool areProductsValid = await CartOrderOperations.CalcualteAndValidateOrderItems(_countryService, _productService, _couponService, _collectionService, _currencyRateService, o, address, coupon, command.Currency);
                _orderService.CreateOrder(o);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    foreach (var item in o.OrderItems)
                        await _orderLogService.ItemCreatedLog(item.Id, o, user.Id);

                    if (areProductsValid)
                    {
                        string image = "https://trgatestore.com/assets/images/logo-text1.png";
                        string message = $"<p><a href='https://www.trgatestore.com'><img style='display: block; margin-left: auto; margin-right: auto;' src='{image}' width='468' height='198'></a></p>" +
                            $"<p style='text-align: center;'><span style='color: rgb(0, 0, 0);'><strong><span style='font-size: 14pt;'>Hello, thanks for choosing Trgate store.</span></strong></span></p>" +
                            $"<p style = 'text-align: center;'><strong><span style = 'color: rgb(45, 194, 107); font-size: 14pt;'> &nbsp; Your order has been completed successfully!</span></strong></p>" +
                            $"<p style = 'text-align: center;'><span style = 'font-size: 14pt;'> Order number is <strong> {o.Id} </strong></span></p>" +
                            $"<p style = 'text-align: center;'><span style = 'font-size: 14pt;'> You can track your order by clicking on the link below</span></p>" +
                            $"<p style = 'text-align: center;'><span style = 'font-size: 18.6667px;'><a title = 'dasd' href = 'https://www.trgatestore.com/en/orders/order-detail-no-account/{o.Id}/{user.Id}'> Track your order!</a> &nbsp;</span></p>" +
                            $"<hr>" +
                            $"<p style = 'text-align: center;'><span style = 'color: rgb(45, 194, 107);'><strong> &nbsp;<span style = 'color: rgb(0, 0, 0);'> Trgate store مرحبا بك, شكرا لاختياركم</span></strong></span></p>" +
                            $"<p style = 'text-align: center;'><span style = 'color: rgb(45, 194, 107);'><strong> &nbsp; !لقد تم قبول طلبك بنجاح</strong></span></ p>" +
                            $"<p style = 'text-align: center;'> رقم الطلب هو <strong>{o.Id}</strong ></p>" +
                            $"<p style = 'text-align: center;'> بمكنك تعقب طلبك بالضغط على الرابط في الأسفل </p>" +
                            $"<p style = 'text-align: center;'><a href = 'https://www.trgatestore.com/en/orders/order-detail-no-account/{o.Id}/{user.Id}'> !تعقب الطلب </a></p> ";
                        //await _emailSender.SendEmailAsync(new string[] { command.Email, }, "Order Confirmed", message, false);
                        if (command.OrderPaymentType == OrderPaymentType.OnDoor)
                        {
                            await _orderLogService.OrderOnDoorPaymentPendingLog(o.Id, user.Id);
                            return new RequestResult<NoAccountPaymentVM>(new NoAccountPaymentVM { OrderId = o.Id, UserId = user.Id });
                        }
                        string myFatoorahError = "";
                        await _orderLogService.OrderCreatedLog(o.Id, user.Id);
                        try
                        {
                            var kwdCurrencyRate = _currencyRateService.GetCurrencyRate("kwd");
                            var totalKwdPrice = o.TotalTLPrice * kwdCurrencyRate;

                            var sendPaymentRequest = CartOrderOperations.GeneratePaymentRequest(o, user, address, totalKwdPrice);
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
                                //x.Data.CustomerReference //order id
                                return new RequestResult<NoAccountPaymentVM>(new NoAccountPaymentVM { PaymentUrl = x.Data.InvoiceURL });
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
                        return new RequestResult<NoAccountPaymentVM>(ActionResultType.BadRequest, "An error occured in payment\nerror: " + myFatoorahError);
                    }

                    return new RequestResult<NoAccountPaymentVM>(ActionResultType.StatusCode634, lang == "en" ? "Some of products is out of stock, your cart has been updated" : "بعض المنتجات نفذت من المستودع، تم تحديث السلة الخاصة بك");
                }

                return new RequestResult<NoAccountPaymentVM>(ActionResultType.BadRequest, lang == "en" ? "An error occured while saving" : "حدث خطأ أثناء الحفظ");



                //return new RequestResult<string>(ActionResultType.BadRequest, "Error");
            }
        }
    }
    public class NoAccountPaymentVM
    {
        public int? OrderId { get; set; }
        public string UserId { get; set; }
        public string PaymentUrl { get; set; }
    }
}
