using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Carts.Command
{
    public class CreateCartItemsCommand : IRequest<RequestResult<bool>>
    {
        public CartItemVM[] Items { get; set; }
        public string? UserId { get; set; }
        public string? Currency { get; set; }
        public string? CountryCode { get; set; }
        public bool ContainsCountryCodeKey { get; set; }
        public class CreateCartItemsCommandHandler : IRequestHandler<CreateCartItemsCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IProductService _productService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICountryService _countryService;
            private readonly ICurrencyRateService _currencyRateService;

            public CreateCartItemsCommandHandler(IUnitOfWork unitOfWork, IProductService productService, IOrderService orderService, ICountryService countryService, ICurrencyRateService currencyRateService)
            {
                _unitOfWork = unitOfWork;
                _productService = productService;
                _orderService = orderService;
                _countryService = countryService;
                _currencyRateService = currencyRateService;
            }

            public async Task<RequestResult<bool>> Handle(CreateCartItemsCommand command, CancellationToken cancellationToken)
            {
                if (command.Items is null || command.Items.Length == 0)
                    return new RequestResult<bool>(true);

                var o = _orderService.Queryable().Include(x => x.OrderItems).OrderByDescending(x => x.Id).AsNoTracking()
                    .FirstOrDefault(x => x.OrderStatus == OrderStatus.CartOrder && x.CustomerId == command.UserId);
                o ??= new Order()
                    {
                        OrderStatus = OrderStatus.CartOrder,
                        CustomerId = command.UserId,
                        OrderItems = new List<OrderItem>(),
                    };

                o.Currency = command.Currency;
                o.Country = command.ContainsCountryCodeKey ? command.CountryCode : _countryService.Queryable().AsNoTracking().FirstOrDefault(x => x.CurrencyCode == o.Currency)?.Code;
                o.CurrencyRate = _currencyRateService.GetCurrencyRate(command.Currency);

                string error = "";
                foreach (var item in command.Items)
                {
                    var oi = o.OrderItems.FirstOrDefault(x => x.ProductId == item.ProductId && (!item.ProductTypeId.HasValue || x.ProductTypeId == item.ProductTypeId));
                    if (oi is null && item.Quantity <= 0)
                        continue;

                    var p = _productService.Queryable().Include(x => x.ProductTypes).AsNoTracking().FirstOrDefault(x => x.Id == item.ProductId);
                    bool productExists = p != null;
                    if (productExists && (p.ProductTypes?.Count > 0 || item.ProductTypeId.HasValue))
                    {
                        if (p.ProductTypes?.Count == 1 && !item.ProductTypeId.HasValue)
                        {
                            item.ProductTypeId = p.ProductTypes.First().Id;

                            oi = o.OrderItems.FirstOrDefault(x => x.ProductId == item.ProductId && (!item.ProductTypeId.HasValue || x.ProductTypeId == item.ProductTypeId));
                            if (oi is null && item.Quantity <= 0)
                                continue;
                        }
                        productExists = p.ProductTypes?.Any(x => x.Id == item.ProductTypeId && x.StockLevel > 0) == true;
                    }
                    if (!productExists)
                    {
                        if (oi != null)
                            oi.Qty = 0;
                        error = "ProductId not found : " + item.ProductId + "\n";
                    }
                    else
                    {
                        if (oi is null)
                        {
                            oi = new OrderItem { ProductId = item.ProductId, ProductTypeId = item.ProductTypeId, Qty = item.Quantity, OrderItemState = OrderItemState.Pending };
                            o.OrderItems.Add(oi);
                        }
                        oi.Qty = item.Quantity;
                    }
                }

                if (o.Id > 0)
                    _orderService.UpdateOrder(o);
                else
                    _orderService.CreateOrder(o);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true, ActionResultType.BadRequest, error);

                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
