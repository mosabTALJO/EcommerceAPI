using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

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
            private readonly IProductService _productService;
            private readonly ICollectionService _collectionService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly ICurrencyRateService _currencyRateService;
            private readonly ICouponService _couponService;
            public GetMyCartNoUserQueryHandler(IOrderService orderService, ICountryService countryService, IProductService productService, IUnitOfWork unitOfWork, IMapper mapper, ICollectionService collectionService, ICurrencyRateService currencyRateService, ICouponService couponService)
            {
                _orderService = orderService;
                _countryService = countryService;
                _productService = productService;
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
                    o.OrderItems.Add(new OrderItem { ProductId = item.ProductId, ProductVariantId = item.ProductVariantId, Qty = item.Quantity, Product = new Product { Id = item.ProductId } });
                }

                await CartOrderOperations.CalcualteCartItems(_countryService, _productService, _orderService, _unitOfWork, _collectionService, _currencyRateService, o, coupon );
                return new RequestResult<OrderVM>(_mapper.Map<OrderVM>(o));
            }
        }
    }
}
