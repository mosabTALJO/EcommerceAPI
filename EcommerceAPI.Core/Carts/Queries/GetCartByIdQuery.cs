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
    public class GetCartByIdQuery : IRequest<RequestResult<OrderVM>>
    {
        public string CustomerID { get; set; }
        public int? OrderId { get; set; }
        public class GetCartByIdQueryHandler : IRequestHandler<GetCartByIdQuery, RequestResult<OrderVM>>
        {
            private readonly IOrderService _orderService;
            private readonly IMapper _mapper;
            private readonly IProductService _productService;
            private readonly ICountryService _countryService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICollectionService _collectionService;
            private readonly ICurrencyRateService _currencyRateService;

            public GetCartByIdQueryHandler(IOrderService orderService, IMapper mapper, IProductService productService, ICountryService countryService, IUnitOfWork unitOfWork, ICollectionService collectionService, ICurrencyRateService currencyRateService)
            {
                _orderService = orderService;
                _mapper = mapper;
                _productService = productService;
                _countryService = countryService;
                _unitOfWork = unitOfWork;
                _collectionService = collectionService;
                _currencyRateService = currencyRateService;
            }

            public async Task<RequestResult<OrderVM>> Handle(GetCartByIdQuery query, CancellationToken cancellationToken)
            {
                var cartOrder = _orderService.Queryable().Include(x => x.OrderItems).AsNoTracking()
               .FirstOrDefault(x => x.OrderStatus == OrderStatus.CartOrder && x.CustomerId == query.CustomerID) ??
                    _orderService.Queryable().Include(x => x.OrderItems).AsNoTracking()
               .FirstOrDefault(x => x.OrderStatus == OrderStatus.CartOrder && x.Id == query.OrderId);

                if (cartOrder is null)
                    return new RequestResult<OrderVM>(new OrderVM());
                var dbOrder = await _orderService.GetOrderDetails(cartOrder.Id);
                if (dbOrder is null)
                    return new RequestResult<OrderVM>(new OrderVM());

                if (string.IsNullOrEmpty(dbOrder.Currency))
                    dbOrder.Currency = "KWD";
                await CartOrderOperations.CalcualteCartItems(_countryService, _productService, _orderService, _unitOfWork, _collectionService, _currencyRateService, dbOrder);

                foreach (var oi in dbOrder.OrderItems)
                {
                    oi.Product.CompanyURL = string.IsNullOrWhiteSpace(oi.Product.CompanyURL) || oi.Product.CompanyURL.StartsWith("https") ? oi.Product.CompanyURL : "https://www.trendyol.com" + (oi.Product.CompanyURL?.StartsWith("/") == true ? "" : "/") + oi.Product.CompanyURL;
                }

                return new RequestResult<OrderVM>(_mapper.Map<OrderVM>(dbOrder));
            }
        }
    }
}
