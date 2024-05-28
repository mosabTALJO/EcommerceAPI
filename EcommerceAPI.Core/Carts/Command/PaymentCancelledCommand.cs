using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Carts.Command
{
    public class PaymentCancelledCommand : IRequest<RequestResult<bool>>
    {
        public string UserId { get; set; }
        public class PaymentCancelledCommandHandler : IRequestHandler<PaymentCancelledCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IUnitOfWork _unitOfWork;

            public PaymentCancelledCommandHandler(IUnitOfWork unitOfWork, IOrderService orderService)
            {
                _unitOfWork = unitOfWork;
                _orderService = orderService;
            }
            public async Task<RequestResult<bool>> Handle(PaymentCancelledCommand command, CancellationToken cancellationToken)
            {
                return new RequestResult<bool>(true);

                
                //var cartOrder = _orderService.Queryable().Include(x => x.OrderItems)
                //.FirstOrDefault(x => x.OrderStatus == OrderStatus.CartOrder && x.CustomerId == command.UserId);
                // if (cartOrder != null)
                //     return new RequestResult<bool>(true);
                // cartOrder = _orderService.Queryable().Include(x => x.OrderItems)
                //     .OrderBy(x => x.Id).LastOrDefault(x => x.OrderStatus == OrderStatus.Pending && x.CustomerId == command.UserId);
                // if (cartOrder is null)
                //     return new RequestResult<bool>(true);

                // cartOrder.OrderStatus = OrderStatus.CartOrder;
                // _orderService.Update(cartOrder);
                // return await _unitOfWork.SaveChangesAsync() > 0 ? new RequestResult<bool>(true) : new RequestResult<bool>(ActionResultType.BadRequest);
            }
        }
    }
}
