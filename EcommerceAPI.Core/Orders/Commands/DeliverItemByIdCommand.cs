using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class DeliverItemByIdCommand : IRequest<RequestResult<bool>>
    {
        public int OrderItemID { get; set; }
        public string UserId { get; set; } = string.Empty;
        public class DeliverItemByIdCommandHandler : IRequestHandler<DeliverItemByIdCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly UserManager<AppUser> _userManager;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICountryService _countryService;

            public DeliverItemByIdCommandHandler(IOrderService orderService, UserManager<AppUser> userManager, IUnitOfWork unitOfWork, ICountryService countryService, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _userManager = userManager;
                _unitOfWork = unitOfWork;
                _countryService = countryService;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(DeliverItemByIdCommand command, CancellationToken cancellationToken)
            {
                var oi = _orderService.ItemsQueryable().Include(x => x.Order)
                 .FirstOrDefault(x => x.Id == command.OrderItemID);
                if (oi is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Item not found");

                if (oi.OrderItemState != OrderItemState.Shipped)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Status error");

                oi.DeliveredDate = DateTime.UtcNow;
                oi.OrderItemState = OrderItemState.Delivered;
                await _orderLogService.ItemDeliveredLog(oi.Id, oi.Order, command.UserId);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
