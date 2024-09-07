using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class AcceptAllItemsCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        AcceptItemCommand[] OrderItems { get; set; } = [];
        public class AcceptAllItemsCommandHandler : IRequestHandler<AcceptAllItemsCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly ICountryService _countryService;
            private readonly IUnitOfWork _unitOfWork;
            public AcceptAllItemsCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, ICountryService countryService, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _countryService = countryService;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(AcceptAllItemsCommand command, CancellationToken cancellationToken)
            {
                var o = _orderService.Queryable()
                 .Include(x => x.OrderItems)
                 .FirstOrDefault(x => x.Id == command.OrderId);
                if (o is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Order not found");
                var country =
                    await _countryService.Queryable().AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode.ToLower() == o.Currency.ToLower()) ??
                    await _countryService.Queryable().AsNoTracking().FirstOrDefaultAsync(x => x.IsDefault);

                foreach (var item in command.OrderItems)
                {
                    var oi = o.OrderItems.FirstOrDefault(x => x.Id == item.OrderItemId);
                    if (oi is null)
                        continue;
                    if (oi.OrderItemState != OrderItemState.Pending && oi.OrderItemState != OrderItemState.Rejected && oi.OrderItemState != OrderItemState.Cancelled)
                        continue;

                    if (oi.OrderItemState == OrderItemState.Rejected || oi.OrderItemState == OrderItemState.Cancelled)
                    {
                        double AddedValue = oi.TotalSalePrice;
                        var totalWithCuponDiscount = o.TotalSalePrice - o.DeliveryPrice + AddedValue;
                        o.SubTotalSalePrice += (oi.SalePrice * oi.Qty);
                        o.DiscountAmount += oi.DiscountAmount;
                        o.DeliveryPrice = OrderOperations.CalculateOrderDeliveryPrice(totalWithCuponDiscount, country);
                        o.TotalSalePrice = totalWithCuponDiscount + o.DeliveryPrice;
                    }
                    await _orderLogService.ItemAcceptedLog(oi.Id, o, command.UserId);
                    _orderService.Accept(oi, item.CostPrice);
                }

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return
                    new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
