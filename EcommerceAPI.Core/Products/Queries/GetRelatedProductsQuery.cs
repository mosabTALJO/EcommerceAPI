using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EcommerceAPI.Core.Products.Queries
{
    public class GetRelatedProductsQuery : IRequest<RequestResult<ListResponse<ProductLiteVM>>>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Currency { get; set; }
        public string ClaimValue { get; set; }
        public class GetRelatedProductsQueryHandler : IRequestHandler<GetRelatedProductsQuery, RequestResult<ListResponse<ProductLiteVM>>>
        {
            private readonly IProductFavoriteService _productFavoriteService;
            private readonly IProductService _productService;
            private readonly ICurrencyRateService _currencyRateService;

            public GetRelatedProductsQueryHandler(IProductService productService, IProductFavoriteService productFavoriteService, ICurrencyRateService currencyRateService)
            {
                _productService = productService;
                _productFavoriteService = productFavoriteService;
                _currencyRateService = currencyRateService;
            }

            public async Task<RequestResult<ListResponse<ProductLiteVM>>> Handle(GetRelatedProductsQuery query, CancellationToken cancellationToken)
            {
                var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";
                var currencyRate = _currencyRateService.GetCurrencyRate(query.Currency);

                var lst = (await _productService.GetRelatedProducts(query.Id, lang, currencyRate).Select(x => new ProductLiteVM
                {
                    Name = lang == "en" ? x.NameEn : lang == "ar" ? x.NameAr : x.NameTr,
                    BrandName = lang == "en" ? x.Brand.NameEn : lang == "ar" ? x.Brand.NameAr : x.Brand.NameTr,
                    SalesPrice = x.CompanyPrice.Value * x.Category.ProfitRatio.Value * currencyRate,
                    Discount = x.Discount > 0 ? x.Discount : x.Category.Discount > 0 ? x.Category.Discount : null,
                    Id = x.Id,
                    MainImageRow = x.MainImage,
                    TotalRate = x.TotalRate,
                    CreatedAt = x.CreatedAt,
                    //VendorId = x.VendorId,
                }).ToArrayAsync()) ?? [];

                var result = new ListResponse<ProductLiteVM>
                {
                    TotalRecords = lst.Length,
                    Items = lst
                };

                if (result.TotalRecords > 0)
                {
                    //calcualte IsFavorite
                    if (!string.IsNullOrEmpty(query.UserId) && query.ClaimValue == "Customer")
                    {
                        var productIds = result.Items.Select(x => x.Id);
                        var exitedProductIds = _productFavoriteService.Queryable().Where(x => productIds.Contains(x.ProductId) && x.AppUserId == query.UserId).Select(x => x.ProductId);
                        foreach (var p in result.Items)
                            p.IsFavorite = exitedProductIds.Contains(p.Id);
                    }
                }
                return new RequestResult<ListResponse<ProductLiteVM>>(result);
            }
        }
    }
}

