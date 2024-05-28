using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class CancelItemCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public int OrderItemID { get; set; }
        public string UserId { get; set; }
        public class CancelItemCommandHandler : IRequestHandler<CancelItemCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly UserManager<AppUser> _userManager;
            private readonly ICountryService _countryService;

            public CancelItemCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ICountryService countryService, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _userManager = userManager;
                _countryService = countryService;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(CancelItemCommand command, CancellationToken cancellationToken)
            {
                var o = _orderService.Queryable()
                 .Include(x => x.Customer)
                 .Include(x => x.OrderItems)
                 .FirstOrDefault(x => x.Id == command.OrderId && x.CustomerId == command.UserId);
                if (o is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Order not found");
                var oi = o.OrderItems.FirstOrDefault(x => x.Id == command.OrderItemID);
                if (oi is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Item not found");
                if (oi.OrderItemState != OrderItemState.Pending)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Status error");

                var country = _countryService.Queryable().AsNoTracking().FirstOrDefault(x => x.CurrencyCode.ToLower() == o.Currency.ToLower()) ??
                        _countryService.Queryable().AsNoTracking().FirstOrDefault(x => x.CurrencyCode.ToLower() == "kwd");

                oi.CancelDate = DateTime.UtcNow;
                oi.OrderItemState = OrderItemState.Cancelled;

                double removedValue = oi.TotalSalePrice;
                var totalWithCuponDiscount = o.TotalSalePrice - o.DeliveryPrice - removedValue;
                o.SubTotalSalePrice -= (oi.SalePrice * oi.Qty);
                o.DiscountAmount -= oi.DiscountAmount;
                o.DeliveryPrice = OrderOperations.CalculateOrderDeliveryPrice(totalWithCuponDiscount, country);
                o.TotalSalePrice = totalWithCuponDiscount + o.DeliveryPrice;
                o.TotalSalePrice = OrderOperations.CheckOrderTotalPriceLessThanZero(o);

                await _orderLogService.ItemCancelledLog(oi.Id, o, command.UserId);
                _orderService.UpdateOrder(o);

                //_orderService.Cancel(oi);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    var u = await _userManager.FindByIdAsync(command.UserId);
                    //TODO based on currency
                    u.CustomerBalance += oi.TotalSalePrice;
                    await _userManager.UpdateAsync(u);
                    return new RequestResult<bool>(true);
                }
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
