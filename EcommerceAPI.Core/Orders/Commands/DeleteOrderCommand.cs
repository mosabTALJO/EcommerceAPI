using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class DeleteOrderCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteOrderCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
            {
                if (await _orderService.FindAsync(command.Id) is Order item)
                {
                    if (item.DeletedAt.HasValue)
                        return new RequestResult<bool>(ActionResultType.BadRequest, "DeletedItem");

                    _orderService.DeleteOrder(item);
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return new RequestResult<bool>(true);
                }
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
