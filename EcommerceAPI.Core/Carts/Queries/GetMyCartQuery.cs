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
    public class GetMyCartQuery : IRequest<RequestResult<OrderVM>>
    {
        public int? AddressId { get; set; }
        public string? UserId { get; set; }
        public string? Currency { get; set; }
        public class GetMyCartQueryHandler : IRequestHandler<GetMyCartQuery, RequestResult<OrderVM>>
        {
            private readonly IOrderService _orderService;
            private readonly ICountryService _countryService;
            private readonly IProductVariantService _productVariantService;
            private readonly ICollectionService _collectionService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly ICurrencyRateService _currencyRateService;

            public GetMyCartQueryHandler(IOrderService orderService, ICountryService countryService, IProductVariantService productVariantService, IUnitOfWork unitOfWork, IMapper mapper, ICollectionService collectionService, ICurrencyRateService currencyRateService)
            {
                _orderService = orderService;
                _countryService = countryService;
                _productVariantService = productVariantService;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _collectionService = collectionService;
                _currencyRateService = currencyRateService;
            }

            public async Task<RequestResult<OrderVM>> Handle(GetMyCartQuery query, CancellationToken cancellationToken)
            {
                var cartOrder = _orderService.Queryable().Include(x => x.OrderItems).AsNoTracking()
                    .FirstOrDefault(x => x.OrderStatus == OrderStatus.CartOrder && x.CustomerId == query.UserId);
                if (cartOrder is null)
                    return new RequestResult<OrderVM>(ActionResultType.NotFound, "No Cart");
                var dbOrder = await _orderService.GetOrderDetails(cartOrder.Id);
                if (dbOrder?.OrderItems?.Any() != true)
                    return new RequestResult<OrderVM>(ActionResultType.NotFound, "No OrderItems");

                dbOrder.Currency = query.Currency ?? dbOrder.Currency;
                if (string.IsNullOrEmpty(dbOrder.Currency))
                    dbOrder.Currency = "TR";

                await CartOrderOperations.CalcualteCartItems(_countryService, _productVariantService, _orderService, _unitOfWork, _collectionService, _currencyRateService, dbOrder);
                return new RequestResult<OrderVM>(_mapper.Map<OrderVM>(dbOrder));
            }
        }
    }
}
