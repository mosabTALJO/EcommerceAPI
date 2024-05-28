using AutoMapper;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Gifts.Commands
{
    public class DeleteGiftPriceRangeCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public class DeleteGiftPriceRangeCommandHandler : IRequestHandler<DeleteGiftPriceRangeCommand, RequestResult<bool>>
        {
            private readonly IGiftPriceRangeService _giftPriceRangeService;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            public DeleteGiftPriceRangeCommandHandler(IGiftPriceRangeService giftPriceRangeService, IMapper mapper, IUnitOfWork unitOfWork)
            {
                _giftPriceRangeService = giftPriceRangeService;
                _mapper = mapper;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(DeleteGiftPriceRangeCommand command, CancellationToken cancellationToken)
            {
                var giftPriceRange = await _giftPriceRangeService.Queryable().FirstOrDefaultAsync(gr => gr.Id == command.Id);
                if (giftPriceRange == null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "NotFound");

                await _giftPriceRangeService.DeleteGiftPriceRange(giftPriceRange);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
