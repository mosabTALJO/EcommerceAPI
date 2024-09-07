using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class ShipOrderCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public int ShippingCompanyId { get; set; }
        public required string TrackingNumber { get; set; }
        public string UserId { get; set; } = string.Empty;
        public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IShippingCompanyService _ShippingCompanyService;
            private readonly IOrderLogService _orderLogService;
            private readonly IUnitOfWork _unitOfWork;

            public ShipOrderCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, IOrderLogService orderLogService, IShippingCompanyService ShippingCompanyService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _orderLogService = orderLogService;
                _ShippingCompanyService = ShippingCompanyService;
            }
            public async Task<RequestResult<bool>> Handle(ShipOrderCommand command, CancellationToken cancellationToken)
            {
                var o = _orderService.Queryable()
                 .Include(x => x.OrderItems)
                 .FirstOrDefault(x => x.Id == command.OrderId);
                if (o is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Order not found");
                var comp = await _ShippingCompanyService.Queryable().FirstOrDefaultAsync(x => x.Id == command.ShippingCompanyId);
                if (comp is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Company not found");

                foreach (var oi in o.OrderItems)
                {
                    if (oi.OrderItemState != OrderItemState.Accepted)
                        continue;
                    await _orderLogService.ItemShippedLog(oi.Id, o, command.UserId);
                    _orderService.ShipItem(oi, comp.NameEn, command.TrackingNumber);
                }

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
