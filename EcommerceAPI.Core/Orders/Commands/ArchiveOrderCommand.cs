using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class ArchiveOrderCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public class ArchiveOrderCommandHnadler : IRequestHandler<ArchiveOrderCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly IUnitOfWork _unitOfWork;

            public ArchiveOrderCommandHnadler(IOrderService orderService, IUnitOfWork unitOfWork, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(ArchiveOrderCommand command, CancellationToken cancellationToken)
            {
                var p = await _orderService.Queryable().FirstOrDefaultAsync(x => x.Id == command.Id);
                if (p is null)
                    return new RequestResult<bool>(ActionResultType.NotFound);
                if (p.IsArchived)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Order already Archived");
                if (p.DeletedAt.HasValue)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "DeletedItem");
                p.IsArchived = true;
                await _orderLogService.OrderUpdatedLog(p, command.UserId, "Order archived");
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
