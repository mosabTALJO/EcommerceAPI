using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class ItemDeliveredCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public int OrderItemID { get; set; }
        public class ItemDeliveredCommandHandler : IRequestHandler<ItemDeliveredCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IUnitOfWork _unitOfWork;

            public ItemDeliveredCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(ItemDeliveredCommand command, CancellationToken cancellationToken)
            {
                var o = _orderService.Queryable()
                 .Include(x => x.Customer)
                 .Include(x => x.OrderItems)
                 .FirstOrDefault(x => x.Id == command.OrderId);
                if (o is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Order not found");
                var oi = o.OrderItems.FirstOrDefault(x => x.Id == command.OrderItemID);
                if (oi is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Item not found");
                if (oi.OrderItemState != OrderItemState.Accepted)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Status error");

                //_orderService.ItemDelivered(oi);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return
                    new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
