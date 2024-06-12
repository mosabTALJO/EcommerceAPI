using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Ratings.Queries
{
    public class GetRatingsQuery : IRequest<RequestResult<ListResponse<OrderRatingVM>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public class GetRatingsQueryHandler : IRequestHandler<GetRatingsQuery, RequestResult<ListResponse<OrderRatingVM>>>
        {
            private readonly IRatingService _ratingService;
            private readonly IProductService _productService;
            private readonly IMapper _mapper;
            public GetRatingsQueryHandler(IRatingService ratingService, IMapper mapper, IProductService productService)
            {
                _ratingService = ratingService;
                _mapper = mapper;
                _productService = productService;
            }

            public async Task<RequestResult<ListResponse<OrderRatingVM>>> Handle(GetRatingsQuery query, CancellationToken cancellationToken)
            {
                IQueryable<int> orderIds = _ratingService.Queryable().AsNoTracking()
                    .Where(x => query.StartDate == null || x.CreatedAt >= query.StartDate)
                    .Where(x => query.EndDate == null || x.CreatedAt <= query.EndDate)
                    .OrderByDescending(r => r.Id)
                    .Select(x => x.OrderId)
                    .Distinct()
                    .Skip(query.PageSize * query.PageNumber).Take(query.PageSize);

                var ratings = await _ratingService.Queryable().AsNoTracking().Include(r => r.Order)
                    .Where(r => orderIds.Contains(r.OrderId))
                    .To<RatingVM>(_mapper)
                    .ToArrayAsync();

                var pIds = ratings.Where(r => r.RatingType == "Product").Select(r => r.RatedItemId).ToArray();

                var products = await _productService.Queryable().AsNoTracking().Include(p => p.Brand)
                    .Where(p => pIds.Contains(p.Id))
                    .ToArrayAsync();

                if (products.Length == 0)
                    return new RequestResult<ListResponse<OrderRatingVM>>(new ListResponse<OrderRatingVM>());

                var temp = new List<OrderRatingVM>();

                foreach (var id in orderIds)
                {
                    var orderRatingVM = new OrderRatingVM
                    {
                        OrderId = id,
                        CreatedAt = ratings.First(r => r.OrderId == id).CreatedAt,
                        CustomerId = ratings.First(r => r.OrderId == id).CustomerId,
                        ServiceRating = ratings.First(r => r.OrderId == id && r.RatingType == "Service").Rate,
                        BrandRatingsAverage = ratings.Where(r => r.OrderId == id && r.RatingType == "Brand").Average(r => r.Rate),
                        ProductRatingsAverage = ratings.Where(r => r.OrderId == id && r.RatingType == "Product").Average(r => r.Rate),
                        Ratings = ratings.Where(r => r.OrderId == id).OrderBy(x => x.RatingType).ToArray()
                    };

                    foreach (var rating in orderRatingVM.Ratings)
                    {
                        if (rating.RatingType == "Product")
                        {
                            if (products.FirstOrDefault(p => p.Id == rating.RatedItemId) != null)
                            {
                                rating.RatedItemName = products?.FirstOrDefault(p => p.Id == rating.RatedItemId).NameEn ?? "";
                                rating.RatedItemPicture = products?.FirstOrDefault(p => p.Id == rating.RatedItemId).MainImage ?? "";
                            }
                        }
                        else if (rating.RatingType == "Brand")
                        {
                            if (products.FirstOrDefault(p => p.BrandId == rating.RatedItemId) != null)
                            {
                                rating.RatedItemName = products?.FirstOrDefault(p => p.BrandId == rating.RatedItemId)?.Brand.NameEn ?? "";
                                rating.RatedItemPicture = products?.FirstOrDefault(p => p.BrandId == rating.RatedItemId)?.Brand.Image ?? "";
                            }
                        }
                    }
                    temp.Add(orderRatingVM);
                }
                var result = new ListResponse<OrderRatingVM>
                {
                    Items = temp.ToArray(),
                    TotalRecords = temp.Count
                };
                return new RequestResult<ListResponse<OrderRatingVM>>(result);
            }
        }
    }
}
