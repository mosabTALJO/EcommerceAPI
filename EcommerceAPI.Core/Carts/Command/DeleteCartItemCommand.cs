using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Carts.Command
{
    public class DeleteCartItemCommand : IRequest<RequestResult<bool>>
    {
        public int CartId { get; set; }
        public int CartItemId { get; set; }
        public class DeleteCartItemCommandHandler : IRequestHandler<DeleteCartItemCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteCartItemCommandHandler(IUnitOfWork unitOfWork, IOrderService orderService)
            {
                _unitOfWork = unitOfWork;
                _orderService = orderService;
            }

            public async Task<RequestResult<bool>> Handle(DeleteCartItemCommand command, CancellationToken cancellationToken)
            {
                var o = _orderService.Queryable()
                 .Include(x => x.Customer)
                 .Include(x => x.OrderItems).AsNoTracking()
                 .FirstOrDefault(x => x.Id == command.CartId && x.OrderStatus == OrderStatus.CartOrder);
                if (o is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Cart not found");
                var oi = o.OrderItems.FirstOrDefault(x => x.Id == command.CartItemId);
                if (oi is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Item not found");
                if (oi.OrderItemState != OrderItemState.Pending)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Status error");

                _orderService.DeleteOrderItem(oi);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);

                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
