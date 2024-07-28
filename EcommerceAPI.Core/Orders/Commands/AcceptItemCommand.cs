using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class AcceptItemCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public string? UserId { get; set; }
        public class AcceptItemCommandHandler : IRequestHandler<AcceptItemCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly ICountryService _countryService;
            private readonly IUnitOfWork _unitOfWork;
            public AcceptItemCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, ICountryService countryService, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _countryService = countryService;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(AcceptItemCommand command, CancellationToken cancellationToken)
            {
                var o = _orderService.Queryable()
                 .Include(x => x.Customer)
                 .Include(x => x.OrderItems)
                 .FirstOrDefault(x => x.Id == command.OrderId);
                if (o is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Order not found");
                var oi = o.OrderItems.FirstOrDefault(x => x.Id == command.OrderItemId);
                if (oi is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Item not found");
                if (oi.OrderItemState != OrderItemState.Pending && oi.OrderItemState != OrderItemState.Rejected && oi.OrderItemState != OrderItemState.Cancelled)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Status error");

                if (oi.OrderItemState == OrderItemState.Rejected || oi.OrderItemState == OrderItemState.Cancelled)
                {
                    var country = await _countryService.Queryable().AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode.ToLower() == o.Currency.ToLower()) ??
                        await _countryService.Queryable().AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode.ToLower() == "kwd");

                    double AddedValue = oi.TotalSalePrice;
                    var totalWithCuponDiscount = o.TotalSalePrice - o.DeliveryPrice + AddedValue;
                    o.SubTotalSalePrice += (oi.SalePrice * oi.Qty);
                    o.DiscountAmount += oi.DiscountAmount;
                    o.DeliveryPrice = OrderOperations.CalculateOrderDeliveryPrice(totalWithCuponDiscount, country);
                    o.TotalSalePrice = totalWithCuponDiscount + o.DeliveryPrice;
                }

                await _orderLogService.ItemAcceptedLog(oi.Id, o, command.UserId);
                _orderService.Accept(oi);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return
                    new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
