using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Carts.Command
{
    public class ConfirmOnDoorPaymentOrderCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public class ConfirmOnDoorPaymentOrderCommandHandler : IRequestHandler<ConfirmOnDoorPaymentOrderCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IUnitOfWork _unitOfWork;
            public ConfirmOnDoorPaymentOrderCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(ConfirmOnDoorPaymentOrderCommand command, CancellationToken cancellationToken)
            {
                var o = await _orderService.Queryable().FirstOrDefaultAsync(x => x.Id == command.OrderId);
                if (o is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "not found");
                if (o.OrderStatus != OrderStatus.OnDoorPaymentPending)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Order Status must be OnDoorPaymentPending");

                o.OrderStatus = OrderStatus.ConfirmedForOnDoorPayment;
                o.PaidSalePrice = o.TotalSalePrice;

                _orderService.Update(o);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "ERROR");
            }
        }
    }
}
