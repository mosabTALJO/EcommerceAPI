using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EcommerceAPI.Core.ProductFavorites.Queries
{
    public class GetFavoriteProductsQuery : IRequest<RequestResult<ProductLiteVM[]>>
    {
        public double CurrencyRate { get; set; }
        public string UserId { get; set; }
        public class GetFavoriteProductsQueryHandler : IRequestHandler<GetFavoriteProductsQuery, RequestResult<ProductLiteVM[]>>
        {
            private readonly IProductFavoriteService _productFavoriteService;
            public GetFavoriteProductsQueryHandler(IProductFavoriteService productFavoriteService)
            {
                _productFavoriteService = productFavoriteService;
            }
            public async Task<RequestResult<ProductLiteVM[]>> Handle(GetFavoriteProductsQuery query, CancellationToken cancellationToken)
            {
                var lst = _productFavoriteService.Queryable().Include(x => x.Product).ThenInclude(x => x.Category)
                .Where(x => x.AppUserId == query.UserId)
                .Select(x => x.Product);

                var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";
                var lstResult = await lst.Where(x => x.Category != null).Select(x => new ProductLiteVM
                {
                    Name = lang == "en" ? x.NameEn : lang == "ar" ? x.NameAr : x.NameTr,
                    BrandName = lang == "en" ? x.Brand.NameEn : lang == "ar" ? x.Brand.NameAr : x.Brand.NameTr,
                    MainImageRow = x.MainImage,
                    Id = x.Id,
                    SalesPrice = x.CompanyPrice.Value * x.Category.ProfitRatio.Value * query.CurrencyRate,
                    Discount = x.Discount > 0 ? x.Discount : x.Category.Discount > 0 ? x.Category.Discount : null,
                    TotalRate = x.TotalRate,
                    CreatedAt = x.CreatedAt,
                    IsFavorite = true,
                }).ToArrayAsync();

                return new RequestResult<ProductLiteVM[]>(lstResult);
            }
        }
    }
}
