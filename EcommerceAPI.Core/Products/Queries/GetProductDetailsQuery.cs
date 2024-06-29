using AutoMapper;
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
                var item = await _productService.Queryable().IgnoreQueryFilters()
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Include(x => x.ProductVariants)
                .Where(x => query.Role == "Admin" || x.IsActive)
                .Where(x => !x.DeletedAt.HasValue).FirstOrDefaultAsync(x => x.Id == query.Id);
                if (item is null)
                    return new RequestResult<ProductVM>(null, ActionResultType.NotFound, "NotFound");

                var p = _mapper.Map<ProductVM>(item);
                //p.CompanyURL = string.IsNullOrWhiteSpace(p.CompanyURL) || p.CompanyURL.StartsWith("https") ? p.CompanyURL : "https://www.trendyol.com" + (p.CompanyURL?.StartsWith("/") == true ? "" : "/") + p.CompanyURL;
                if (string.IsNullOrEmpty(p.NameEn)) p.NameEn = p.NameTr;
                if (string.IsNullOrEmpty(p.NameAr)) p.NameAr = p.NameEn;

                CleanImages(p, query.Role);

                //calcualte IsFavorite
                if (!string.IsNullOrEmpty(query.UserId))
                    p.IsFavorite = _productFavoriteService.Queryable().FirstOrDefault(x => x.ProductId == p.Id && x.AppUserId == query.UserId) != null;

                //p.SalesPrice = p.CompanyPrice.Value * p.Category.ProfitRatio.Value * _currencyRateService.GetCurrencyRate(query.Currency);
                p.Discount = p.Discount > 0 ? p.Discount : p.Category.Discount > 0 ? p.Category.Discount : null;
                p.Category.FullName = p.Category.NameEn;

                if (query.Role != "Admin")
                {
                    p.Discount = p.Discount > 0 ? p.Discount : p.Category.Discount > 0 ? p.Category.Discount : null;
                }
                return new RequestResult<ProductVM>(p);
            }

            private void CleanImages(ProductVM p, string role)
            {
                //if (p.VendorId == Vendor.Custom)
                //{
                //    if (role != "Admin")
                //    {
                //        p.MainImage = $"https://api.trgatestore.com/api/Files/PreviewImage/{p.MainImage}?w=700&h=700";
                //        for (int i = 0; i < (p.Images?.Count ?? 0); i++)
                //            p.Images[i] = $"https://api.trgatestore.com/api/Files/PreviewImage/{p.Images[i]}?w=700&h=700&crop=false";
                //    }
                //    return;
                //}

                if (string.IsNullOrWhiteSpace(p.MainImage))
                {
                    if (p.Images?.Any() == true)
                        p.MainImage = p.Images[0].StartsWith("https") ? p.Images[0] : $"https://cdn.dsmcdn.com/mnresize/{1200}/{1000}" + p.Images[0];
                }
                else
                {
                    p.MainImage = p.MainImage.StartsWith("https") ? p.MainImage : $"https://cdn.dsmcdn.com/mnresize/{1200}/{1000}" + p.MainImage;
                    p.Images.Insert(0, p.MainImage);
                }
                p.Images = p.Images.Where(x => !string.IsNullOrEmpty(x)).Select(x => x.StartsWith("https") ? x : $"https://cdn.dsmcdn.com/mnresize/{1200}/{1000}" + x).ToList();
                p.Images = p.Images?.Distinct().ToList();
            }
        }
    }
}
