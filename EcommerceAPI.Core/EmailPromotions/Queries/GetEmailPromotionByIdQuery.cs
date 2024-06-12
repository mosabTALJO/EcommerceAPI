using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.EmailPromotions.Queries
{
    public class GetEmailPromotionByIdQuery : IRequest<RequestResult<EmailPromotionVM>>
    {
        public int Id { get; set; }
        public class GetEmailPromotionByIdQueryHandler : IRequestHandler<GetEmailPromotionByIdQuery, RequestResult<EmailPromotionVM>>
        {
            private readonly IEmailPromotionService _emailPromotionService;
            private readonly IMapper _mapper;

            public GetEmailPromotionByIdQueryHandler(IEmailPromotionService emailPromotionService, IMapper mapper)
            {
                _emailPromotionService = emailPromotionService;
                _mapper = mapper;
            }
            public async Task<RequestResult<EmailPromotionVM>> Handle(GetEmailPromotionByIdQuery query, CancellationToken cancellationToken)
            {
                var epVM = await _emailPromotionService.Queryable().AsNoTracking().To<EmailPromotionVM>(_mapper).FirstOrDefaultAsync(x => x.Id == query.Id);
                if (epVM == null)
                    return new RequestResult<EmailPromotionVM>(ActionResultType.NotFound);
                return new RequestResult<EmailPromotionVM>(epVM);
            }
        }
    }
}
