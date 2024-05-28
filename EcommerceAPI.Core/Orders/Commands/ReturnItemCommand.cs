using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class ReturnItemCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public int OrderItemID { get; set; }
        public string ReturnReason { get; set; }
        public string UserId { get; set; }
        public class ReturnItemCommandHandler : IRequestHandler<ReturnItemCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly IUnitOfWork _unitOfWork;

            public ReturnItemCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(ReturnItemCommand command, CancellationToken cancellationToken)
            {
                var o = _orderService.Queryable()
                 .Include(x => x.Customer)
                 .Include(x => x.OrderItems)
                 .FirstOrDefault(x => x.Id == command.OrderId && x.CustomerId == command.UserId);
                if (o is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Order not found");
                var oi = o.OrderItems.FirstOrDefault(x => x.Id == command.OrderItemID);
                if (oi is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Item not found");

                await _orderLogService.ItemReturningLog(oi.Id, o, command.UserId);
                _orderService.Return(oi, command.ReturnReason);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return new RequestResult<bool>(true);
                }
                return
                    new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
