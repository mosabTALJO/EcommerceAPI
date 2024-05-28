using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Coupons.Queries
{
    public class GetUserCouponsQuery : IRequest<RequestResult<CouponVM[]>>
    {
        public string UserId { get; set; }
        public class GetUserCouponsQueryHandler : IRequestHandler<GetUserCouponsQuery, RequestResult<CouponVM[]>>
        {
            private readonly ICouponService _couponService;
            private readonly IMapper _mapper;
            public GetUserCouponsQueryHandler(IMapper mapper, ICouponService couponService)
            {
                _mapper = mapper;
                _couponService = couponService;
            }
            public async Task<RequestResult<CouponVM[]>> Handle(GetUserCouponsQuery query, CancellationToken cancellationToken)
            {
                return new RequestResult<CouponVM[]>(await _couponService.Queryable().AsNoTracking()
                    .Where(x => x.CustomerId == query.UserId)
                    .Where(x => x.IsActive)
                    .Where(x => x.ExpireDate > DateTime.UtcNow)
                    .To<CouponVM>(_mapper).ToArrayAsync());
            }
        }
    }
}
