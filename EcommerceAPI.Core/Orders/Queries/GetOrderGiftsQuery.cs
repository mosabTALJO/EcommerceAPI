using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Services;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Core.Extentions;

namespace EcommerceAPI.Core.Orders.Queries
{
    public class GetOrderGiftsQuery : IRequest<RequestResult<List<GiftVM>>>
    {
        public string CustomerId { get; set; }
        public class GetOrderGiftsQueryHandler : IRequestHandler<GetOrderGiftsQuery, RequestResult<List<GiftVM>>>
        {
            private readonly IOrderService _orderService;
            private readonly IMapper _mapper;
            private readonly IGiftPriceRangeService _giftPriceRangeService;
            private readonly ICouponService _couponService;
            private readonly IUnitOfWork _unitOfWork;
            public GetOrderGiftsQueryHandler(IOrderService orderService, IMapper mapper, IGiftPriceRangeService giftPriceRangeService, ICouponService couponService, IUnitOfWork unitOfWork)
            {
                _orderService = orderService;
                _mapper = mapper;
                _giftPriceRangeService = giftPriceRangeService;
                _couponService = couponService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<List<GiftVM>>> Handle(GetOrderGiftsQuery query, CancellationToken cancellationToken)
            {
                var order = await _orderService.Queryable().OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync(o => o.CustomerId == query.CustomerId && o.OrderStatus > OrderStatus.Pending && !o.DeletedAt.HasValue);

                if (order == null)
                    return new RequestResult<List<GiftVM>>(new List<GiftVM>());
                if (order.GiftId.HasValue && order.IsGiftTaken == true)
                    return new RequestResult<List<GiftVM>>(new List<GiftVM>());

                var giftRange = await _giftPriceRangeService.Queryable().Include(gr => gr.Country)
                    .To<GiftPriceRangeVM>(_mapper)
                    .FirstOrDefaultAsync(gr => (gr.Country.Code.ToLower() == order.Country.ToLower() || gr.Country.NameEn.ToLower() == order.Country.ToLower() || gr.Country.NameTr.ToLower() == order.Country.ToLower() || gr.Country.NameAr.ToLower() == order.Country.ToLower()) && gr.MinPrice <= order.TotalSalePrice && gr.MaxPrice >= order.TotalSalePrice);
                if (giftRange == null)
                    return new RequestResult<List<GiftVM>>(new List<GiftVM>());

                var availableGifts = new List<GiftVM>();
                foreach (var gift in giftRange.Gifts)
                {
                    if (gift.Quantity > 0 && gift.IsActive)
                        availableGifts.Add(gift);
                }
                if (!availableGifts.Any())
                    return new RequestResult<List<GiftVM>>(new List<GiftVM>());

                GiftVM wonGift;
                if (order.GiftId.HasValue)
                {
                    wonGift = availableGifts.FirstOrDefault(x => x.Id == order.GiftId.Value);
                    if (wonGift is null)
                        wonGift = giftRange.Gifts.FirstOrDefault(x => x.Id == order.GiftId.Value);

                    if (wonGift != null)
                        wonGift.IsSelected = true;
                }
                else
                {
                    var randomIndex = new Random().Next(availableGifts.Count - 1);
                    wonGift = availableGifts.ElementAtOrDefault(randomIndex);
                    wonGift.IsSelected = true;
                    wonGift.Quantity--;
                    if (wonGift.GiftType == GiftType.Coupon)
                    {
                        var coupon = await _couponService.CreateCustomerCoupon(wonGift.CouponDiscount, order.CustomerId);
                        wonGift.CouponId = coupon.Id;
                    }
                    order.GiftId = wonGift.Id;
                    _orderService.Update(order);
                    await _unitOfWork.SaveChangesAsync();
                }
                if (wonGift is null)
                    return new RequestResult<List<GiftVM>>(new List<GiftVM>());

                var availableGiftsCount = availableGifts.Count;
                for (int i = 0; i < 30 - availableGiftsCount; i++)
                    availableGifts.Add(new GiftVM { Image = "aaa.png", });

                return new RequestResult<List<GiftVM>>(availableGifts);
            }          
            
            
            public async Task<RequestResult<List<GiftVM>>> Handle_OLD(GetOrderGiftsQuery query, CancellationToken cancellationToken)
            {
                var order = await _orderService.Queryable().OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync(o => o.CustomerId == query.CustomerId && o.OrderStatus > OrderStatus.Pending && !o.DeletedAt.HasValue);

                if (order == null)
                    return new RequestResult<List<GiftVM>>(ActionResultType.NotFound, "order not found");
                if (order.GiftId.HasValue && order.IsGiftTaken == true)
                    return new RequestResult<List<GiftVM>>(ActionResultType.NotFound, "order has already been gifted");

                var giftRange = await _giftPriceRangeService.Queryable().Include(gr => gr.Country)
                    .To<GiftPriceRangeVM>(_mapper)
                    .FirstOrDefaultAsync(gr => (gr.Country.Code.ToLower() == order.Country.ToLower() || gr.Country.NameEn.ToLower() == order.Country.ToLower() || gr.Country.NameTr.ToLower() == order.Country.ToLower() || gr.Country.NameAr.ToLower() == order.Country.ToLower()) && gr.MinPrice <= order.TotalSalePrice && gr.MaxPrice >= order.TotalSalePrice);
                if (giftRange == null)
                    return new RequestResult<List<GiftVM>>(ActionResultType.NotFound, "no gifts found for this price");

                var availableGifts = new List<GiftVM>();
                foreach (var gift in giftRange.Gifts)
                {
                    if (gift.Quantity > 0 && gift.IsActive)
                        availableGifts.Add(gift);
                }
                if (!availableGifts.Any())
                    return new RequestResult<List<GiftVM>>(ActionResultType.NotFound, "no gifts found for this price");

                GiftVM wonGift;
                if (order.GiftId.HasValue)
                {
                    wonGift = availableGifts.FirstOrDefault(x => x.Id == order.GiftId.Value);
                    if (wonGift is null)
                        wonGift = giftRange.Gifts.FirstOrDefault(x => x.Id == order.GiftId.Value);

                    if (wonGift != null)
                        wonGift.IsSelected = true;
                }
                else
                {
                    var randomIndex = new Random().Next(availableGifts.Count - 1);
                    wonGift = availableGifts.ElementAtOrDefault(randomIndex);
                    wonGift.IsSelected = true;
                    wonGift.Quantity--;
                    if (wonGift.GiftType == GiftType.Coupon)
                    {
                        var coupon = await _couponService.CreateCustomerCoupon(wonGift.CouponDiscount, order.CustomerId);
                        wonGift.CouponId = coupon.Id;
                    }
                    order.GiftId = wonGift.Id;
                    _orderService.Update(order);
                    await _unitOfWork.SaveChangesAsync();
                }
                if (wonGift is null)
                    return new RequestResult<List<GiftVM>>(ActionResultType.NotFound, "no gifts found for this priceeeeeeeeeeeee");

                var availableGiftsCount = availableGifts.Count;
                for (int i = 0; i < 30 - availableGiftsCount; i++)
                    availableGifts.Add(new GiftVM { Image = "aaa.png", });

                return new RequestResult<List<GiftVM>>(availableGifts);
            }
        }
    }
}
