using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.ShippingCompanies.Queries
{
    public class GetAllShippingCompaniesnQuery : IRequest<ListResponse<ShippingCompanyVM>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public class GetAllShippingCompaniesnQueryHandler : IRequestHandler<GetAllShippingCompaniesnQuery, ListResponse<ShippingCompanyVM>>
        {
            private readonly IShippingCompanyService _ShippingCompanyService;
            private readonly IMapper _mapper;
            public GetAllShippingCompaniesnQueryHandler(IShippingCompanyService ShippingCompanyService, IMapper mapper)
            {
                _ShippingCompanyService = ShippingCompanyService;
                _mapper = mapper;
            }
            public async Task<ListResponse<ShippingCompanyVM>> Handle(GetAllShippingCompaniesnQuery query, CancellationToken cancellationToken)
            {
                query.PageNumber = query.PageNumber.PagingNumber();
                var companies = _ShippingCompanyService.Queryable().AsNoTracking();

                return new ListResponse<ShippingCompanyVM>
                {
                    Items = await companies.Skip(query.PageSize * query.PageNumber).Take(query.PageSize).To<ShippingCompanyVM>(_mapper).ToArrayAsync(),
                    TotalRecords = companies.Count()
                };
            }
        }
    }
}
