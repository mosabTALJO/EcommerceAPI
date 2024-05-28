using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class ChangeDeliveryPriceCommand : IRequest<RequestResult<bool>>
    {
        public string UserId { get; set; }
        public int OrderId { get; set; }
        public double NewDeliveryPrice { get; set; }
        public class ChangeDeliveryPriceCommandHandler : IRequestHandler<ChangeDeliveryPriceCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly IUnitOfWork _unitOfWork;
            public ChangeDeliveryPriceCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(ChangeDeliveryPriceCommand command, CancellationToken cancellationToken)
            {
                var order = await _orderService.Queryable().FirstOrDefaultAsync(x => x.Id == command.OrderId);
                if (order == null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "not found");

                string note = $"DeliveryPrice changed from {order.DeliveryPrice} to {command.NewDeliveryPrice}" +
                            $"\nTotalSalePrice changed from {order.TotalSalePrice} to {order.TotalSalePrice - (order.DeliveryPrice - command.NewDeliveryPrice)}";

                order.TotalSalePrice -= (order.DeliveryPrice - command.NewDeliveryPrice);
                order.DeliveryPrice = command.NewDeliveryPrice;
                order.TotalSalePrice = OrderOperations.CheckOrderTotalPriceLessThanZero(order);

                _orderService.Update(order);
                if (order.DeliveryPrice != command.NewDeliveryPrice)
                    await _orderLogService.OrderUpdatedLog(order, command.UserId, note);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "ERROR");
            }
        }
    }
}