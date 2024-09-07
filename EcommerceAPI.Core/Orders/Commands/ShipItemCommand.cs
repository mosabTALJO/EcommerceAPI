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
        public int OrderItemId { get; set; }
        public int ShippingCompanyId { get; set; }
        public required string TrackingNumber { get; set; }
        public string UserId { get; set; }
        public class ShipItemCommandHandler : IRequestHandler<ShipItemCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IKargoCompanyService _kargoCompanyService;
            private readonly IOrderLogService _orderLogService;
            private readonly IUnitOfWork _unitOfWork;

            public ShipItemCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, IOrderLogService orderLogService, IKargoCompanyService kargoCompanyService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _orderLogService = orderLogService;
                _kargoCompanyService = kargoCompanyService;
            }
            public async Task<RequestResult<bool>> Handle(ShipItemCommand command, CancellationToken cancellationToken)
            {
                var oi = _orderService.ItemsQueryable().Include(x => x.Order)
                    .FirstOrDefault(x => x.Id == command.OrderItemId);
                if (oi is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Order not found");
                var comp = await _kargoCompanyService.Queryable().FirstOrDefaultAsync(x => x.Id == command.ShippingCompanyId);
                if (comp is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Company not found");

                if (oi.OrderItemState != OrderItemState.Accepted)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Status Error");

                await _orderLogService.ItemShippedLog(oi.Id, oi.Order, command.UserId);
                _orderService.ShipItem(oi, comp.NameEn, command.TrackingNumber);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
