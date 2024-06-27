using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.EmailPromotions.Queries
{
    public class GetAllEmailPromotionsQuery : IRequest<RequestResult<ListResponse<EmailPromotionLiteVM>>>
    {
        public string? Subject { get; set; } = null;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public class GetAllEmailPromotionsQueryHandler : IRequestHandler<GetAllEmailPromotionsQuery, RequestResult<ListResponse<EmailPromotionLiteVM>>>
        {
            private readonly IEmailPromotionService _emailPromotionService;
            private readonly IMapper _mapper;

            public GetAllEmailPromotionsQueryHandler(IEmailPromotionService emailPromotionService, IMapper mapper)
            {
                _emailPromotionService = emailPromotionService;
                _mapper = mapper;
            }

            public async Task<RequestResult<ListResponse<EmailPromotionLiteVM>>> Handle(GetAllEmailPromotionsQuery query, CancellationToken cancellationToken)
            {
                query.PageNumber = query.PageNumber.PagingNumber();
                var result = _emailPromotionService.Queryable().OrderByDescending(x => x.CreatedAt).AsNoTracking().Where(x => string.IsNullOrEmpty(query.Subject) || x.Subject.Contains(query.Subject))
                            .To<EmailPromotionLiteVM>(_mapper);

                return new RequestResult<ListResponse<EmailPromotionLiteVM>>(new ListResponse<EmailPromotionLiteVM>
                {
                    Items = await result.Skip(query.PageSize * query.PageNumber).Take(query.PageSize).ToArrayAsync(),
                    TotalRecords = result.Count()
                });
            }
        }
    }
}
