using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class CancelAllItemsCommand : IRequest<RequestResult<bool>>
    {
        public string UserId { get; set; }
        public int OrderId { get; set; }
        public class CancelAllItemsCommandHandler : IRequestHandler<CancelAllItemsCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly UserManager<AppUser> _userManager;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICountryService _countryService;
            public CancelAllItemsCommandHandler(IOrderService orderService, UserManager<AppUser> userManager, IUnitOfWork unitOfWork, ICountryService countryService, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _userManager = userManager;
                _unitOfWork = unitOfWork;
                _countryService = countryService;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(CancelAllItemsCommand command, CancellationToken cancellationToken)
            {
                var o = _orderService.Queryable()
                 .Include(x => x.Customer)
                 .Include(x => x.OrderItems)
                 .FirstOrDefault(x => x.Id == command.OrderId);
                if (o is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Order not found");
                if (o.Currency == null)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "order currency code is null");

                if (o.OrderItems.Any(x => x.OrderItemState != OrderItemState.Pending))
                    return new RequestResult<bool>(ActionResultType.BadRequest, "not all items are pending");

                var u = await _userManager.FindByIdAsync(command.UserId);

                var country = _countryService.Queryable().AsNoTracking().FirstOrDefault(x => x.CurrencyCode.ToLower() == o.Currency.ToLower()) ??
                                _countryService.Queryable().AsNoTracking().FirstOrDefault(x => x.CurrencyCode.ToLower() == "kwd");

                double removedValue = 0;
                double removedSubValue = 0;
                double removedDiscountValue = 0;
                foreach (var oi in o.OrderItems)
                {
                    oi.CancelDate = DateTime.UtcNow;
                    oi.OrderItemState = OrderItemState.Cancelled;

                    await _orderLogService.ItemCancelledLog(oi.Id, o, command.UserId);

                    removedValue += oi.TotalSalePrice;
                    removedSubValue += (oi.SalePrice * oi.Qty);
                    removedDiscountValue += oi.DiscountAmount;
                }

                var totalWithCuponDiscount = o.TotalSalePrice - o.DeliveryPrice - removedValue;
                o.SubTotalSalePrice -= removedSubValue;
                o.DiscountAmount -= removedDiscountValue;
                o.DeliveryPrice = OrderOperations.CalculateOrderDeliveryPrice(totalWithCuponDiscount, country);
                o.TotalSalePrice = totalWithCuponDiscount + o.DeliveryPrice;
                o.TotalSalePrice = OrderOperations.CheckOrderTotalPriceLessThanZero(o);

                _orderService.UpdateOrder(o);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    //TODO based on currency
                    if (u != null)
                        u.CustomerBalance += removedValue;
                    await _userManager.UpdateAsync(u);
                    return new RequestResult<bool>(true);
                }
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
