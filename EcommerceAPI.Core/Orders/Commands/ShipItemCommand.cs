using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class ShipItemCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public int OrderItemID { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingCode { get; set; }
        public string UserId { get; set; }
        public class ShipItemCommandHandler : IRequestHandler<ShipItemCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly IUnitOfWork _unitOfWork;

            public ShipItemCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(ShipItemCommand command, CancellationToken cancellationToken)
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

                //await _orderLogService.ShipItemLog(oi.Id, command.UserId);
                _orderService.ShipItem(oi, command.ShippingCompany, command.ShippingCode);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return
                    new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
