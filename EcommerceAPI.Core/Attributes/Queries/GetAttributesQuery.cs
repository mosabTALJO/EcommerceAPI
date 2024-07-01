using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Attributes.Queries
{
    public class GetAttributesQuery : IRequest<RequestResult<ProductAttributeVM[]>>
    {
        public string Search { get; set; } = string.Empty;
        public class GetAttributesQueryHandler : IRequestHandler<GetAttributesQuery, RequestResult<ProductAttributeVM[]>>
        {
            private readonly IAttributesService _attributesService;
            private readonly IMapper _mapper;
            public GetAttributesQueryHandler(IAttributesService attributesService, IMapper mapper)
            {
                _attributesService = attributesService;
                _mapper = mapper;
            }
            public async Task<RequestResult<ProductAttributeVM[]>> Handle(GetAttributesQuery request, CancellationToken cancellationToken) =>
                new RequestResult<ProductAttributeVM[]>(await _attributesService.Queryable()
                    .Where(x => x.NameAr.Contains(request.Search) || x.NameEn.Contains(request.Search) || x.NameTr.Contains(request.Search))
                    .To<ProductAttributeVM>(_mapper).ToArrayAsync());
        }
    }
}
