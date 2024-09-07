using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class DeliverAllItemsCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public class DeliverAllItemsCommandHandler : IRequestHandler<DeliverAllItemsCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly UserManager<AppUser> _userManager;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICountryService _countryService;

            public DeliverAllItemsCommandHandler(IOrderService orderService, UserManager<AppUser> userManager, IUnitOfWork unitOfWork, ICountryService countryService, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _userManager = userManager;
                _unitOfWork = unitOfWork;
                _countryService = countryService;
                _orderLogService = orderLogService;
            }

            public async Task<RequestResult<bool>> Handle(DeliverAllItemsCommand command, CancellationToken cancellationToken)
            {
                var o = _orderService.Queryable()
                 .Include(x => x.Customer)
                 .Include(x => x.OrderItems)
                 .FirstOrDefault(x => x.Id == command.OrderId);
                if (o is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Order not found");

                var u = await _userManager.FindByIdAsync(command.UserId);
                foreach (var oi in o.OrderItems)
                {
                    if (oi.OrderItemState != OrderItemState.Shipped)
                        continue;

                    oi.DeliveredDate = DateTime.UtcNow;
                    oi.OrderItemState = OrderItemState.Delivered;
                    await _orderLogService.ItemDeliveredLog(oi.Id, o, command.UserId);
                }

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
