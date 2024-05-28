using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Gifts.Commands
{
    public class UpdateGiftPriceRangeCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public ICollection<GiftPostVM> Gifts { get; set; }
        public class UpdateGiftPriceRangeCommandHandler : IRequestHandler<UpdateGiftPriceRangeCommand, RequestResult<bool>>
        {
            private readonly IGiftPriceRangeService _giftPriceRangeService;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            public UpdateGiftPriceRangeCommandHandler(IGiftPriceRangeService giftPriceRangeService, IMapper mapper, IUnitOfWork unitOfWork)
            {
                _giftPriceRangeService = giftPriceRangeService;
                _mapper = mapper;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(UpdateGiftPriceRangeCommand command, CancellationToken cancellationToken)
            {
                var giftPriceRange = await _giftPriceRangeService.Queryable().Include(gr => gr.Gifts).FirstOrDefaultAsync(gr => gr.Id == command.Id);
                if (giftPriceRange == null) 
                    return new RequestResult<bool>(ActionResultType.NotFound, "NotFound");

                giftPriceRange.MinPrice = command.MinPrice;
                giftPriceRange.MaxPrice = command.MaxPrice;

                var gifts = new List<Gift>();
                foreach (var gift in command.Gifts)
                    gifts.Add(_mapper.Map<Gift>(gift));

                await _giftPriceRangeService.UpdateGiftPriceRange(giftPriceRange, gifts);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
