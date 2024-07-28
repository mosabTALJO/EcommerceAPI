using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Carts.Queries
{
    public class GetMyCartNoUserQuery : IRequest<RequestResult<OrderVM>>
    {
        public string? Currency { get; set; }
        public string? CouponCode { get; set; }
        public CartItemVM[] Items { get; set; }
        public class GetMyCartNoUserQueryHandler : IRequestHandler<GetMyCartNoUserQuery, RequestResult<OrderVM>>
        {
            private readonly IOrderService _orderService;
            private readonly ICountryService _countryService;
            private readonly IProductVariantService _productVariantService;
            private readonly ICollectionService _collectionService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly ICurrencyRateService _currencyRateService;
            private readonly ICouponService _couponService;
            public GetMyCartNoUserQueryHandler(IOrderService orderService, ICountryService countryService, IProductVariantService productVariantService, IUnitOfWork unitOfWork, IMapper mapper, ICollectionService collectionService, ICurrencyRateService currencyRateService, ICouponService couponService)
            {
                _orderService = orderService;
                _countryService = countryService;
                _productVariantService = productVariantService;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _collectionService = collectionService;
                _currencyRateService = currencyRateService;
                _couponService = couponService;
            }

            public async Task<RequestResult<OrderVM>> Handle(GetMyCartNoUserQuery query, CancellationToken cancellationToken)
            {
                Coupon? coupon = null;
                if (!string.IsNullOrEmpty(query.CouponCode))
                {
                    coupon = _couponService.CheckCoupon(query.CouponCode, out string couponError);
                    if (coupon == null)
                        return new RequestResult<OrderVM>(ActionResultType.NotFound, "orders.coupon-not-found");
                }

                var o = new Order
                {
                    OrderItems = [],
                    Currency = query.Currency ?? "KWD"
                };


                foreach (var item in query.Items)
                {
                    var pv = await _productVariantService.Queryable().Where(x => x.Id == item.ProductVariantId)
                     .Select(x => new { x.Id, x.ProductId, x.Name, x.Product.MainImage }).FirstOrDefaultAsync();
                    if (pv != null)
                    {
                        var oi = new OrderItem
                        {
                            ProductVariantId = item.ProductVariantId,
                            Qty = item.Quantity,
                            ProductName = pv.Name,
                            ProductImage = pv.MainImage,
                            OrderItemState = OrderItemState.Pending,
                        };
                        o.OrderItems.Add(oi);
                    }
                }

                await CartOrderOperations.CalcualteCartItems(_countryService, _productVariantService, _orderService, _unitOfWork, _collectionService, _currencyRateService, o, coupon);
                return new RequestResult<OrderVM>(_mapper.Map<OrderVM>(o));
            }
        }
    }
}
