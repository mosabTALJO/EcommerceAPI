using AutoMapper;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using MediatR;

namespace EcommerceAPI.Core.Attributes.Queries
{
    public class GetAttributeValuesQuery : IRequest<RequestResult<string[]>>
    {
        public int Id { get; set; }
        public class GetAttributesByIdQueryHandler : IRequestHandler<GetAttributeValuesQuery, RequestResult<string[]>>
        {
            private readonly IAttributesService _attributesService;
            private readonly IMapper _mapper;
            public GetAttributesByIdQueryHandler(IAttributesService attributesService, IMapper mapper)
            {
                _attributesService = attributesService;
                _mapper = mapper;
            }
            public async Task<RequestResult<string[]>> Handle(GetAttributeValuesQuery request, CancellationToken cancellationToken) =>
                new RequestResult<string[]>(await _attributesService.GetAttribiteValues(request.Id));
        }
    }
}
