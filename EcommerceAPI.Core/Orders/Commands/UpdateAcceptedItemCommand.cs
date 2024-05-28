using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class UpdateAcceptedItemCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public string ProductUrl { get; set; }
        public string OrderNumber { get; set; }
        public double SalePriceNoKDV { get; set; }
        public double KDV { get; set; }
        public string UserId { get; set; }
        public class UpdateAcceptedItemCommandHandler : IRequestHandler<UpdateAcceptedItemCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly IUnitOfWork _unitOfWork;
            public UpdateAcceptedItemCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(UpdateAcceptedItemCommand command, CancellationToken cancellationToken)
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
                if (oi.OrderItemState != OrderItemState.Accepted)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Status error");


                bool addLog = false;
                string logNotes = $"Update Accepted Item:";//\nold SalePriceNoKDV: {oi.SalePriceNoKDV}\nold ProductUrl: {oi.ProductUrl}\nold OrderNumber: {oi.OrderNumber}\nold KDV: {oi.KDV}";
                if (oi.SalePriceNoKDV != command.SalePriceNoKDV)
                {
                    logNotes += "\nOld Price=" + oi.SalePriceNoKDV;
                    addLog = true;
                }
                if (oi.ProductUrl != command.ProductUrl)
                {
                    logNotes += "\nOld URL=" + oi.ProductUrl;
                    addLog = true;
                }
                if (oi.OrderNumber != command.OrderNumber)
                {
                    logNotes += "\nOld OrderNumber=" + oi.OrderNumber;
                    addLog = true;
                }
                if (oi.KDV != command.KDV)
                {
                    logNotes += "\nOld KDV=" + oi.KDV;
                    addLog = true;
                }

                oi.SalePriceNoKDV = command.SalePriceNoKDV;
                oi.ProductUrl = command.ProductUrl;
                oi.OrderNumber = command.OrderNumber;
                oi.KDV = command.KDV;

                if (addLog)
                    await _orderLogService.ItemUpdatedOrEditedLog(oi, command.UserId, logNotes);
                _orderService.UpdateOrder(o);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return
                    new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
