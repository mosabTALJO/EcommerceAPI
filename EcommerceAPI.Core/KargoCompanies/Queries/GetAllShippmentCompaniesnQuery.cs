using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.ShippmentCompanies.Queries
{
    public class GetAllShippmentCompaniesnQuery : IRequest<ListResponse<ShippmentCompanyVM>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public class GetAllShippmentCompaniesnQueryHandler : IRequestHandler<GetAllShippmentCompaniesnQuery, ListResponse<ShippmentCompanyVM>>
        {
            private readonly IShippmentCompanyService _shippmentCompanyService;
            private readonly IMapper _mapper;
            public GetAllShippmentCompaniesnQueryHandler(IShippmentCompanyService shippmentCompanyService, IMapper mapper)
            {
                _shippmentCompanyService = shippmentCompanyService;
                _mapper = mapper;
            }
            public async Task<ListResponse<ShippmentCompanyVM>> Handle(GetAllShippmentCompaniesnQuery query, CancellationToken cancellationToken)
            {
                query.PageNumber = query.PageNumber.PagingNumber();
                var companies = _shippmentCompanyService.Queryable().AsNoTracking();

                return new ListResponse<ShippmentCompanyVM>
                {
                    Items = await companies.Skip(query.PageSize * query.PageNumber).Take(query.PageSize).To<ShippmentCompanyVM>(_mapper).ToArrayAsync(),
                    TotalRecords = companies.Count()
                };
            }
        }
    }
}
