using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class RejectItemByIdCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public int OrderItemID { get; set; }
        public string UserId { get; set; }
        public class RejectItemByIdCommandHandler : IRequestHandler<RejectItemByIdCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly UserManager<AppUser> _userManager;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICountryService _countryService;

            public RejectItemByIdCommandHandler(IOrderService orderService, UserManager<AppUser> userManager, IUnitOfWork unitOfWork, ICountryService countryService, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _userManager = userManager;
                _unitOfWork = unitOfWork;
                _countryService = countryService;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(RejectItemByIdCommand command, CancellationToken cancellationToken)
            {
                var o = _orderService.Queryable()
                 .Include(x => x.Customer)
                 .Include(x => x.OrderItems)
                 .FirstOrDefault(x => x.Id == command.OrderId);
                if (o is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Order not found");
                if (o.Currency == null)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "order currency code is null");

                var oi = o.OrderItems.FirstOrDefault(x => x.Id == command.OrderItemID);
                if (oi is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Item not found");
                if (oi.OrderItemState != OrderItemState.Pending && oi.OrderItemState != OrderItemState.Accepted)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Status error");

                var country = _countryService.Queryable().AsNoTracking().FirstOrDefault(x => x.CurrencyCode.ToLower() == o.Currency.ToLower()) ??
                        _countryService.Queryable().AsNoTracking().FirstOrDefault(x => x.CurrencyCode.ToLower() == "kwd");
               
                oi.RejectedDate = DateTime.UtcNow;
                oi.OrderItemState = OrderItemState.Rejected;

                double removedValue =  oi.TotalSalePrice;
                var totalWithCuponDiscount = o.TotalSalePrice - o.DeliveryPrice - removedValue;
                o.SubTotalSalePrice -= (oi.SalePrice * oi.Qty);
                o.DiscountAmount -= oi.DiscountAmount;
                o.DeliveryPrice = OrderOperations.CalculateOrderDeliveryPrice(totalWithCuponDiscount, country);
                o.TotalSalePrice = totalWithCuponDiscount + o.DeliveryPrice;
                o.TotalSalePrice = OrderOperations.CheckOrderTotalPriceLessThanZero(o);

                await _orderLogService.ItemRejectedLog(oi.Id, o, command.UserId);
                _orderService.UpdateOrder(o);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
