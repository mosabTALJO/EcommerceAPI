using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class ApplyGiftToOrderCommand : IRequest<RequestResult<bool>>
    {
        public string CustomerId { get; set; }
        public class ApplyGiftToOrderCommandHandler : IRequestHandler<ApplyGiftToOrderCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IMapper _mapper;
            private readonly IGiftPriceRangeService _giftPriceRangeService;
            private readonly IUnitOfWork _unitOfWork;
            public ApplyGiftToOrderCommandHandler(IOrderService orderService, IMapper mapper, IGiftPriceRangeService giftPriceRangeService, IUnitOfWork unitOfWork)
            {
                _orderService = orderService;
                _mapper = mapper;
                _giftPriceRangeService = giftPriceRangeService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(ApplyGiftToOrderCommand command, CancellationToken cancellationToken)
            {
                var order = await _orderService.Queryable().OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync(o => o.CustomerId == command.CustomerId && o.OrderStatus > OrderStatus.Pending && !o.DeletedAt.HasValue);
                if (order == null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "order not found");

                if (!order.GiftId.HasValue)
                    return new RequestResult<bool>(ActionResultType.NotFound, "this order doesn't have a gift");

                if (order.IsGiftTaken)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "this order has already been gifted");

                order.IsGiftTaken = true;
                _orderService.Update(order);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "ERROR");
            }
        }
    }
}
