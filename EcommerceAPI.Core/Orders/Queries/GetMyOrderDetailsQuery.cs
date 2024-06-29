using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Services;
using EcommerceAPI.Core.Extentions;

namespace EcommerceAPI.Core.Orders.Queries
{
    public class GetMyOrderDetailsQuery : IRequest<RequestResult<OrderVM>>
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public class GetMyOrderDetailsQueryHandler : IRequestHandler<GetMyOrderDetailsQuery, RequestResult<OrderVM>>
        {
            private readonly IOrderService _orderService;
            private readonly IMapper _mapper;
            private readonly IRatingService _ratingService;

            public GetMyOrderDetailsQueryHandler(IOrderService orderService, IMapper mapper, IRatingService ratingService)
            {
                _orderService = orderService;
                _mapper = mapper;
                _ratingService = ratingService;
            }

            public async Task<RequestResult<OrderVM>> Handle(GetMyOrderDetailsQuery query, CancellationToken cancellationToken)
            {
                var item = await _orderService.GetOrderDetails(query.OrderId);
                if (item is null)
                    return new RequestResult<OrderVM>(ActionResultType.NotFound, "NotFound");
                if (item.CustomerId != query.UserId)
                    return new RequestResult<OrderVM>(ActionResultType.NotFound, "NotFound");
                var o = _mapper.Map<OrderVM>(item);

                foreach (var oi in o.OrderItems)
                {
                    oi.Currency = item.Currency;
                    //oi.Product.CompanyURL = string.IsNullOrWhiteSpace(oi.Product.CompanyURL) || oi.Product.CompanyURL.StartsWith("https") ? oi.Product.CompanyURL : "https://www.trendyol.com" + (oi.Product.CompanyURL?.StartsWith("/") == true ? "" : "/") + oi.Product.CompanyURL;
                }
                o.Ratings = await _ratingService.Queryable().AsNoTracking().Where(r => r.OrderId == o.Id).To<RatingVM>(_mapper).ToArrayAsync();
                return new RequestResult<OrderVM>(o);
            }
        }
    }
}
