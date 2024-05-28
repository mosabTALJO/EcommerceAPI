using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class AddSpecialDiscountCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public double SpecialDiscountAmount { get; set; }
        public string UserId { get; set; }
        public class AddSpecialDiscountCommandHandler : IRequestHandler<AddSpecialDiscountCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly IUnitOfWork _unitOfWork;
            public AddSpecialDiscountCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(AddSpecialDiscountCommand command, CancellationToken cancellationToken)
            {
                var order = await _orderService.Queryable().FirstOrDefaultAsync(x => x.Id == command.OrderId);
                if (order == null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "not found");

                var oldTotalSalePrice = order.TotalSalePrice;
                order.TotalSalePrice += order.SpecialDiscount;
                order.SpecialDiscount = command.SpecialDiscountAmount;
                order.TotalSalePrice -= order.SpecialDiscount;
                order.TotalSalePrice = OrderOperations.CheckOrderTotalPriceLessThanZero(order);

                string note = $"SpecialDiscount = {command.SpecialDiscountAmount}" +
                              $"\norder TotalSalePrice changed from {oldTotalSalePrice} to {order.TotalSalePrice}";

                if (order.SpecialDiscount > 0)
                {
                    await _orderLogService.OrderUpdatedLog(order, command.UserId, note);
                    _orderService.Update(order);
                }
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "ERROR");

            }
        }
    }
}
