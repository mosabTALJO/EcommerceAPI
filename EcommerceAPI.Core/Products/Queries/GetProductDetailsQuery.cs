using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Products.Queries
{
    public class GetProductDetailsQuery : IRequest<RequestResult<ProductVM>>
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }
        public string Currency { get; set; }
        public class GetProductDetailsQueryHandler : IRequestHandler<GetProductDetailsQuery, RequestResult<ProductVM>>
        {
            private readonly IProductService _productService;
            private readonly IMapper _mapper;
            private readonly IProductFavoriteService _productFavoriteService;
            private readonly ICurrencyRateService _currencyRateService;

            public GetProductDetailsQueryHandler(IProductService productService, IMapper mapper, IProductFavoriteService productFavoriteService, ICurrencyRateService currencyRateService)
            {
                _productService = productService;
                _mapper = mapper;
                _productFavoriteService = productFavoriteService;
                _currencyRateService = currencyRateService;
            }
            public async Task<RequestResult<ProductVM>> Handle(GetProductDetailsQuery query, CancellationToken cancellationToken)
            {
                var usdRate = _currencyRateService.GetUSDRate(query.Currency, true);

                var p = await _productService.Queryable().IgnoreQueryFilters()
                .Where(x => query.Role == "Admin" || x.IsActive)
                .Where(x => !x.DeletedAt.HasValue)
                .Include(x => x.ProductVariants.Where(x => x.IsActive))
                .To<ProductVM>(_mapper)
                .FirstOrDefaultAsync(x => x.Id == query.Id);
                if (p == null)
                    return new RequestResult<ProductVM>(null, ActionResultType.NotFound, "NotFound");

                if (string.IsNullOrEmpty(p.NameEn)) p.NameEn = p.NameTr;
                if (string.IsNullOrEmpty(p.NameAr)) p.NameAr = p.NameEn;

                CleanImages(p, query.Role);

                //calcualte IsFavorite
                if (!string.IsNullOrEmpty(query.UserId))
                    p.IsFavorite = _productFavoriteService.Queryable().FirstOrDefault(x => x.ProductId == p.Id && x.AppUserId == query.UserId) != null;
                p.Category.FullName = p.Category.NameEn;

                if (query.Role != "Admin")
                {
                    p.Discount = p.Discount > 0 ? p.Discount : p.Category.Discount > 0 ? p.Category.Discount : null;
                    p.SalesPrice = query.Currency == p.Currency ? p.SalesPrice : p.SalesPriceUSD * usdRate;
                    foreach (var pv in p.ProductVariants)
                    {
                        pv.SalesPrice = query.Currency == p.Currency ? pv.SalesPrice : pv.SalesPriceUSD * usdRate;
                    }
                }
                
                p.ProductVariants = p.ProductVariants.Where(x => x.IsActive).ToList();

                return new RequestResult<ProductVM>(p);
            }
            private void CleanImages(ProductVM p, string role)
            {
                if (string.IsNullOrWhiteSpace(p.MainImage))
                {
                    if (p.Images.Count > 0)
                        p.MainImage = p.Images[0];
                }
                else
                {
                    p.MainImage = p.MainImage;
                    p.Images.Insert(0, p.MainImage);
                }
                p.Images = p.Images.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            }
        }
    }
}