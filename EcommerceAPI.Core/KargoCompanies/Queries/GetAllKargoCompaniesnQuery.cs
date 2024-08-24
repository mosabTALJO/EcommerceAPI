using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.KargoCompanies.Queries
{
    public class GetAllKargoCompaniesnQuery : IRequest<ListResponse<KargoCompanyVM>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public class GetAllKargoCompaniesnQueryHandler : IRequestHandler<GetAllKargoCompaniesnQuery, ListResponse<KargoCompanyVM>>
        {
            private readonly IKargoCompanyService _kargoCompanyService;
            private readonly IMapper _mapper;
            public GetAllKargoCompaniesnQueryHandler(IKargoCompanyService kargoCompanyService, IMapper mapper)
            {
                _kargoCompanyService = kargoCompanyService;
                _mapper = mapper;
            }
            public async Task<ListResponse<KargoCompanyVM>> Handle(GetAllKargoCompaniesnQuery query, CancellationToken cancellationToken)
            {
                var companies = _kargoCompanyService.Queryable().AsNoTracking();

                return new ListResponse<KargoCompanyVM>
                {
                    Items = await companies.Skip(query.PageSize * query.PageNumber).Take(query.PageSize).To<KargoCompanyVM>(_mapper).ToArrayAsync(),
                    TotalRecords = companies.Count()
                };
            }
        }
    }
}
