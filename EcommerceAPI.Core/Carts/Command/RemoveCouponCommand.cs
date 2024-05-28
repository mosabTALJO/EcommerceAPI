using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Carts.Command
{
    public class RemoveCouponCommand : IRequest<RequestResult<bool>>
    {
        public string UserId { get; set; }
        public class RemoveCouponCommandHandler : IRequestHandler<RemoveCouponCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IUnitOfWork _unitOfWork;

            public RemoveCouponCommandHandler(IUnitOfWork unitOfWork, IOrderService orderService)
            {
                _unitOfWork = unitOfWork;
                _orderService = orderService;
            }

            public async Task<RequestResult<bool>> Handle(RemoveCouponCommand command, CancellationToken cancellationToken)
            {
                var o = _orderService.Queryable().Include(x => x.OrderItems)
                .FirstOrDefault(x => x.OrderStatus == OrderStatus.CartOrder && x.CustomerId == command.UserId);

                if (o?.DiscountCoupunId.HasValue == true)
                {
                    foreach (var oi in o.OrderItems)
                        oi.CoupunDiscount = null;

                    o.CoupunDiscountRate = null;
                    o.DiscountCoupunId = null;
                    _orderService.UpdateOrder(o);
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return new RequestResult<bool>(true);
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
                }

                return new RequestResult<bool>(true);
            }
        }
    }
}
