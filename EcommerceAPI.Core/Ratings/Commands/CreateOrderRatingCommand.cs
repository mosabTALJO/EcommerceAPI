using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Ratings.Commands
{
    public class CreateOrderRatingCommand : IRequest<RequestResult<bool>>
    {
        public string UserId { get; set; }
        public int OrderId { get; set; }
        public RatingPostVM[] BrandsRates { get; set; }
        public RatingPostVM[] ProductsRates { get; set; }
        public RatingPostVM ServiceRate { get; set; }
        public class CreateOrderRatingCommandHandler : IRequestHandler<CreateOrderRatingCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IRatingService _ratingService;
            private readonly IUnitOfWork _unitOfWork;
            public CreateOrderRatingCommandHandler(IOrderService orderService, IRatingService ratingService, IUnitOfWork unitOfWork)
            {
                _orderService = orderService;
                _ratingService = ratingService;
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestResult<bool>> Handle(CreateOrderRatingCommand command, CancellationToken cancellationToken)
            {
                var order = await _orderService.Queryable().AsNoTracking().Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == command.OrderId);
                if (order == null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "order not found");
                if (order.CustomerId != command.UserId)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "you don't own this order");

                var orderRating = await _ratingService.Queryable().FirstOrDefaultAsync(r => r.OrderId == order.Id);
                if (orderRating != null)
                    _ratingService.Delete(orderRating);
                if (await _unitOfWork.SaveChangesAsync() < 0)
                    return new RequestResult<bool>(ActionResultType.BadRequest, "Error");

                _ratingService.Insert(new Rating
                {
                    RatingType = RatingType.Service,
                    OrderId = order.Id,
                    Rate = command.ServiceRate.Rate,
                    Description = command.ServiceRate.Description
                });

                foreach (var rating in command.ProductsRates)
                {
                    if (order.OrderItems.Any(oi => oi.ProductId != rating.RatedItemId))
                        return new RequestResult<bool>(ActionResultType.BadRequest, "product not found in order");

                    _ratingService.Insert(new Rating
                    {
                        RatingType = RatingType.Product,
                        OrderId = order.Id,
                        Rate = rating.Rate,
                        Description = rating.Description,
                        RatedItemId = rating.RatedItemId
                    });
                }

                foreach (var rating in command.BrandsRates)
                {
                    _ratingService.Insert(new Rating
                    {
                        RatingType = RatingType.Brand,
                        OrderId = order.Id,
                        Rate = rating.Rate,
                        Description = rating.Description,
                        RatedItemId = rating.RatedItemId
                    });
                }

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
