using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Products.Queries
{
    public class GetProductVariantsQuery : IRequest<RequestResult<ProductVariantVM[]>>
    {
        public int Id { get; set; }
        public class GetProductTypesQueryHandler : IRequestHandler<GetProductVariantsQuery, RequestResult<ProductVariantVM[]>>
        {
            private readonly IProductService _productService;
            private readonly IMapper _mapper;

            public GetProductTypesQueryHandler(IProductService productService, IMapper mapper)
            {
                _productService = productService;
                _mapper = mapper;
            }
            public async Task<RequestResult<ProductVariantVM[]>> Handle(GetProductVariantsQuery query, CancellationToken cancellationToken)
            {
                var items = await _productService.GetProductVariant(query.Id).To<ProductVariantVM>(_mapper).ToArrayAsync();
                if (items is null)
                    return new RequestResult<ProductVariantVM[]>(null, ActionResultType.NotFound);

                return new RequestResult<ProductVariantVM[]>(items);
            }
        }
    }
}
