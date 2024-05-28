using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Gifts.Commands
{
    public class CreateGiftPriceRangeCommand : IRequest<RequestResult<int>>
    {
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public int CountryId { get; set; }
        public ICollection<GiftPostVM> Gifts { get; set; }
        public class CreateGiftPriceRangeCommandHandler : IRequestHandler<CreateGiftPriceRangeCommand, RequestResult<int>>
        {
            private readonly IGiftPriceRangeService _giftPriceRangeService;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            public CreateGiftPriceRangeCommandHandler(IGiftPriceRangeService giftPriceRangeService, IMapper mapper, IUnitOfWork unitOfWork)
            {
                _giftPriceRangeService = giftPriceRangeService;
                _mapper = mapper;
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestResult<int>> Handle(CreateGiftPriceRangeCommand command, CancellationToken cancellationToken)
            {
                var giftPriceRange = _mapper.Map<GiftPriceRange>(command);
                await _giftPriceRangeService.CreateGiftPriceRange(giftPriceRange);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(giftPriceRange.Id);
                return new RequestResult<int>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
