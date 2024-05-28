using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class UnArchiveOrderCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public class UnArchiveOrderCommandHnadler : IRequestHandler<UnArchiveOrderCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly IUnitOfWork _unitOfWork;

            public UnArchiveOrderCommandHnadler(IOrderService orderService, IUnitOfWork unitOfWork, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(UnArchiveOrderCommand command, CancellationToken cancellationToken)
            {
                var p = await _orderService.Queryable().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == command.Id);
                if (p is null)
                    return new RequestResult<bool>(ActionResultType.NotFound);
                if (!p.IsArchived)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Order already unarchived");
                if (p.DeletedAt.HasValue)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "DeletedItem");
                p.IsArchived = false;
                await _orderLogService.OrderUpdatedLog(p, command.UserId, "Order unarchived");
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}