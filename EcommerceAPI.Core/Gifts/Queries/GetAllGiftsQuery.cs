using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Gifts.Queries
{
    public class GetAllGiftsQuery : IRequest<RequestResult<ListResponse<GiftPriceRangeVM>>>
    {
        public int CountryId { get; set; }
        public class GetAllGiftsQueryHandler : IRequestHandler<GetAllGiftsQuery, RequestResult<ListResponse<GiftPriceRangeVM>>>
        {
            private readonly IGiftPriceRangeService _giftPriceRangeService;
            private readonly IMapper _mapper;
            public GetAllGiftsQueryHandler(IGiftPriceRangeService giftService, IMapper mapper)
            {
                _giftPriceRangeService = giftService;
                _mapper = mapper;
            }

            public async Task<RequestResult<ListResponse<GiftPriceRangeVM>>> Handle(GetAllGiftsQuery query, CancellationToken cancellationToken)
            {
                var giftRanges = _giftPriceRangeService.Queryable().Include(gr => gr.Gifts).Where(gr => gr.CountryId == query.CountryId).To<GiftPriceRangeVM>(_mapper);
                if (giftRanges == null || giftRanges.Count() == 0)
                    return new RequestResult<ListResponse<GiftPriceRangeVM>>(ActionResultType.NotFound, "no gifts found for this country");

                var result = new ListResponse<GiftPriceRangeVM>
                {
                    TotalRecords = giftRanges.Count(),
                    Items = giftRanges.ToArray()
                };

                return new RequestResult<ListResponse<GiftPriceRangeVM>>(result);
            }
        }
    }
}
