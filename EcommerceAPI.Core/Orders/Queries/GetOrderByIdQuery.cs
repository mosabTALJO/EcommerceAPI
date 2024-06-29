using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Core.Extentions;

namespace EcommerceAPI.Core.Orders.Queries
{
    public class GetOrderByIdQuery : IRequest<RequestResult<OrderVM>>
    {
        public int Id { get; set; }
        public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, RequestResult<OrderVM>>
        {
            private readonly IOrderService _orderService;
            private readonly IMapper _mapper;
            private readonly ICouponService _couponService;
            private readonly IRatingService _ratingService;
            private readonly UserManager<AppUser> _userManager;
            public GetOrderByIdQueryHandler(IOrderService orderService, IMapper mapper, ICouponService couponService, IRatingService ratingService, UserManager<AppUser> userManager)
            {
                _orderService = orderService;
                _mapper = mapper;
                _couponService = couponService;
                _ratingService = ratingService;
                _userManager = userManager;
            }
            public async Task<RequestResult<OrderVM>> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
            {
                var item = await _orderService.GetOrderDetails(query.Id);
                if (item is null)
                    return new RequestResult<OrderVM>(ActionResultType.NotFound, "NotFound");
                var o = _mapper.Map<OrderVM>(item);

                if (o.DiscountCoupunId.HasValue)
                    o.CoupunName = _couponService.Queryable().FirstOrDefault(x => x.Id == o.DiscountCoupunId)?.NameEn;

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
