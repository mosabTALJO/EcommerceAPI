using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Carts.Command
{
    public class CreateCratOrderCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public string PaymentId { get; set; }
        public string InvoiceId { get; set; }
        public string CustomerName { get; set; }
        public double TotalSalePrice { get; set; }
        public double DeliveryPrice { get; set; }
        public double? PaidSalePrice { get; set; }
        public int? DiscountCoupunId { get; set; }
        public double? CoupunDiscountRate { get; set; }
        public string CoupunName { get; set; }
        public string Currency { get; set; }
        public double CurrencyRate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public OrderPaymentType PaymentType { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public string FullAddress { get; set; }
        public string RecipientFirstName { get; set; }
        public string RecipientLastName { get; set; }
        public string PhoneNumber { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PaymentGateway? PaymentGateway { get; set; }
        public ICollection<OrderItemVM> OrderItems { get; set; }
        public double SubTotalSalePrice { get; set; }
        public string GeneratedPaymentLinks { get; set; }
        public bool IsArchived { get; set; }
        public string UserId { get; set; }
        public class CreateCratOrderCommandHandler : IRequestHandler<CreateCratOrderCommand, RequestResult<bool>>
        {
            private readonly ICouponService _couponService;
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly ICountryService _countryService;
            private readonly IProductVariantService _productVariantService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICollectionService _collectionService;
            private readonly ICurrencyRateService _currencyRateService;

            public CreateCratOrderCommandHandler(IOrderService orderService, ICouponService couponService, ICountryService countryService, IUnitOfWork unitOfWork, IProductVariantService productVariantService, ICollectionService collectionService, IOrderLogService orderLogService, ICurrencyRateService currencyRateService)
            {
                _orderService = orderService;
                _couponService = couponService;
                _countryService = countryService;
                _unitOfWork = unitOfWork;
                _productVariantService = productVariantService;
                _collectionService = collectionService;
                _orderLogService = orderLogService;
                _currencyRateService = currencyRateService;
            }

            public async Task<RequestResult<bool>> Handle(CreateCratOrderCommand command, CancellationToken cancellationToken)
            {
                var cartOrder = _orderService.Queryable().Include(x => x.OrderItems).AsNoTracking()
                .FirstOrDefault(x => x.OrderStatus == OrderStatus.CartOrder && x.Id == command.Id);
                if (cartOrder?.OrderItems?.Any() != true)
                    return new RequestResult<bool>(ActionResultType.NotFound, "No items in the cart");

                var o = await _orderService.GetOrderDetails(cartOrder.Id);

                Coupon coupon = null;
                if (o.DiscountCoupunId.HasValue)
                {
                    coupon = _couponService.CheckCoupon(o.DiscountCoupunId.Value, out string error);
                    if (coupon is null)
                        return new RequestResult<bool>(ActionResultType.BadRequest, "Coupun Error: " + error);
                }
                o.OrderStatus = OrderStatus.OnDoorPaymentPending;
                o.PaymentType = OrderPaymentType.OnDoor;
                o.OrderDeviceType = OrderDeviceType.DashBoard;

                var country = _countryService.Queryable().FirstOrDefault(x => x.Id == command.CountryId);
                var address = new UserAddress
                {
                    CountryId = country.Id,
                    FirstName = command.RecipientFirstName,
                    LastName = command.RecipientLastName,
                    CityId = command.CityId,
                    PhoneNumber = command.PhoneNumber,
                    FullAddress = command.FullAddress,
                };

                bool areProductsValid = await CartOrderOperations.CalcualteAndValidateOrderItems(_countryService, _productVariantService, _couponService, _collectionService, _currencyRateService, o, address, coupon, country.CurrencyCode);

                _orderService.UpdateOrder(o, true);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    await _orderLogService.OrderOnDoorPaymentPendingLog(o.Id, command.UserId);
                    foreach (var item in o.OrderItems)
                        await _orderLogService.ItemCreatedLog(item.Id, o, command.UserId);

                    if (areProductsValid)
                        return new RequestResult<bool>(true);

                    return new RequestResult<bool>(ActionResultType.StatusCode634, "Some of the products are out of stock, your cart has been updated");
                }

                return new RequestResult<bool>(ActionResultType.BadRequest, "An error occured while saving");
            }
        }
    }
}
