using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Brands.Queries
{
    public class GetBrandByIdQuery : IRequest<RequestResult<BrandVM>>
    {
        public int Id { get; set; }
        public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, RequestResult<BrandVM>>
        {
            private readonly IBrandService _brandService;
            private readonly IMapper _mapper;
            public GetBrandByIdQueryHandler(IBrandService brandService, IMapper mapper)
            {
                _brandService = brandService;
                _mapper = mapper;
            }

            public async Task<RequestResult<BrandVM>> Handle(GetBrandByIdQuery query, CancellationToken cancellationToken)
            {
                if (await _brandService.Queryable().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == query.Id) is Brand b)
                    return new RequestResult<BrandVM>(_mapper.Map<BrandVM>(b));
                return new RequestResult<BrandVM>(ActionResultType.NotFound, "IdError");
            }
        }
    }
}
