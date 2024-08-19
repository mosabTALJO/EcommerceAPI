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
        public string Currency { get; set; } = string.Empty;
        public class GetProductTypesQueryHandler : IRequestHandler<GetProductVariantsQuery, RequestResult<ProductVariantVM[]>>
        {
            private readonly IProductService _productService;
            private readonly IMapper _mapper;
            private readonly ICurrencyRateService _currencyRateService;

            public GetProductTypesQueryHandler(IProductService productService, IMapper mapper, ICurrencyRateService currencyRateService)
            {
                _productService = productService;
                _mapper = mapper;
                _currencyRateService = currencyRateService;
            }
            public async Task<RequestResult<ProductVariantVM[]>> Handle(GetProductVariantsQuery query, CancellationToken cancellationToken)
            {
                var usdRate = 1 / _currencyRateService.GetUSDRate(query.Currency);

                var items = await _productService.GetProductVariant(query.Id).Select(x => new ProductVariantVM
                {
                    Barcode = x.Barcode,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    Id = x.Id,
                    Images = x.Images.Split(',', StringSplitOptions.None).ToList(),
                    Discount = x.Discount,
                    FullDescription = x.FullDescription,
                    ProductId = x.ProductId,
                    SalesPrice = query.Currency == x.Product.Currency ? x.SalesPrice : x.SalesPriceUSD * usdRate,
                    SalesPriceUSD = x.SalesPriceUSD,
                    SKU = x.SKU,
                    StockLevel = x.StockLevel,

                }).ToArrayAsync();
                if (items is null)
                    return new RequestResult<ProductVariantVM[]>(null, ActionResultType.NotFound);

                return new RequestResult<ProductVariantVM[]>(items);
            }
        }
    }
}
