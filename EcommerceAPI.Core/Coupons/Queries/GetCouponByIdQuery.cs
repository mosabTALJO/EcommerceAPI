using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Coupons.Queries
{
    public class GetCouponByIdQuery : IRequest<RequestResult<CouponVM>>
    {
        public int CouponId { get; set; }
        public class GetCouponByIdQueryHandler : IRequestHandler<GetCouponByIdQuery, RequestResult<CouponVM>>
        {
            private readonly IOrderService _orderService;
            private readonly ICouponService _couponService;
            private readonly IMapper _mapper;

            public GetCouponByIdQueryHandler(IOrderService orderService, ICouponService couponService, IMapper mapper)
            {
                _orderService = orderService;
                _couponService = couponService;
                _mapper = mapper;
            }

            public async Task<RequestResult<CouponVM>> Handle(GetCouponByIdQuery query, CancellationToken cancellationToken)
            {
                var coupon = await _couponService.Queryable().FirstOrDefaultAsync(c => c.Id == query.CouponId);
                if (coupon is null)
                    return new RequestResult<CouponVM>(ActionResultType.NotFound, "NotFound");

                var lst = await _orderService.Queryable().AsNoTracking().Where(x => !x.DeletedAt.HasValue)
                    .Where(o => o.DiscountCoupunId == query.CouponId).ToArrayAsync();

                var couponVM = _mapper.Map<CouponVM>(coupon);
                couponVM.CustomersCount = lst.Count();
                couponVM.PaidCustomersCount = lst.Where(o => o.OrderStatus > 0).Count();
                couponVM.TotalSales = lst.Where(o => o.OrderStatus > OrderStatus.Pending).Sum(o => o.TotalSalePrice /*/ o.CurrencyRate*/);

                return new RequestResult<CouponVM>(couponVM);
            }
        }
    }
}
