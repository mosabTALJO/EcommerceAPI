using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class TogglePaymentTypeCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public class ChangeToOnlineCommandHandler : IRequestHandler<TogglePaymentTypeCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly IUnitOfWork _unitOfWork;

            public ChangeToOnlineCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _orderLogService = orderLogService;
            }

            public async Task<RequestResult<bool>> Handle(TogglePaymentTypeCommand command, CancellationToken cancellationToken)
            {
                var order = await _orderService.Queryable().FirstOrDefaultAsync(x => x.Id == command.OrderId);
                if (order == null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "order not found");

                string newPaymentType = "";
                if (order.PaymentType == OrderPaymentType.OnDoor)
                {
                    order.PaymentType = OrderPaymentType.Online;
                    newPaymentType = OrderPaymentType.Online.ToString();
                }
                
                else if (order.PaymentType == OrderPaymentType.Online)
                {
                    order.PaymentType = OrderPaymentType.OnDoor;
                    newPaymentType = OrderPaymentType.OnDoor.ToString();
                }

                _orderService.Update(order);
                await _orderLogService.OrderUpdatedLog(order, command.UserId, $"Payment type changed to {newPaymentType}");

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "ERROR");
            }
        }
    }
}
