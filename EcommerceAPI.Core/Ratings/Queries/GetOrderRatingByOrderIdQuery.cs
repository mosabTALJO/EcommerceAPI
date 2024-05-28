using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace TrGate.Core.Ratings.Queries
{
    public class GetOrderRatingByOrderIdQuery : IRequest<RequestResult<List<RatingVM>>>
    {
        public int OrderId { get; set; }
        public class GetOrderRatingByOrderIdQueryHandler : IRequestHandler<GetOrderRatingByOrderIdQuery, RequestResult<List<RatingVM>>>
        {
            private readonly IRatingService _ratingService;
            private readonly IMapper _mapper;

            public GetOrderRatingByOrderIdQueryHandler(IRatingService ratingService, IMapper mapper)
            {
                _ratingService = ratingService;
                _mapper = mapper;
            }
            public async Task<RequestResult<List<RatingVM>>> Handle(GetOrderRatingByOrderIdQuery query, CancellationToken cancellationToken)
            {
                var orderRatings = await _ratingService.Queryable().AsNoTracking().Where(r => r.OrderId == query.OrderId).To<RatingVM>(_mapper).ToListAsync();
                if (!orderRatings.Any())
                    return new RequestResult<List<RatingVM>>(ActionResultType.NotFound, "no ratings found");

                return new RequestResult<List<RatingVM>>(orderRatings);
            }
        }
    }
}