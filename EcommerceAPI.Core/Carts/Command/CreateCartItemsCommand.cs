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
            private readonly IProductVariantService _productVariantService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICountryService _countryService;
            private readonly ICurrencyRateService _currencyRateService;

            public CreateCartItemsCommandHandler(IUnitOfWork unitOfWork, IProductVariantService productVariantService, IOrderService orderService, ICountryService countryService, ICurrencyRateService currencyRateService)
            {
                _unitOfWork = unitOfWork;
                _productVariantService = productVariantService;
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
                //o.CurrencyRate = _currencyRateService.GetCurrencyRate(command.Currency);

                string error = "";
                foreach (var item in command.Items)
                {
                    var oi = o.OrderItems.FirstOrDefault(x => x.ProductVariantId == item.ProductVariantId);
                    if (oi is null && item.Quantity <= 0)
                        continue;

                    var pv = await _productVariantService.Queryable().Where(x => x.Id == item.ProductVariantId)
                        .Select(x => new { x.Id, x.ProductId }).FirstOrDefaultAsync();
                    if (pv == null)
                    {
                        if (oi != null)
                            oi.Qty = 0;
                        error = "ProductVariantId not found : " + item.ProductVariantId + "\n";
                    }
                    else
                    {
                        if (oi is null)
                        {
                            oi = new OrderItem { ProductVariantId = item.ProductVariantId, Qty = item.Quantity, OrderItemState = OrderItemState.Pending };
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
                    return new RequestResult<bool>(true, ActionResultType.OK, error);

                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
