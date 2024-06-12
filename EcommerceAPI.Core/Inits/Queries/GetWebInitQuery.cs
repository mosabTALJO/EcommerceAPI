using AutoMapper;
using EcommerceAPI.Core.BaseItems;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace EcommerceAPI.Core.Inits.Queries
{
    public class GetWebInitQuery : IRequest<RequestResult<WebInitData>>
    {
        public string UserId { get; set; }
        public string Role { get; set; }
        public string Currency { get; set; }
        public string lang { get; set; }
        public double CurrencyRate { get; set; }
        public class GetWebInitQueryHandler : IRequestHandler<GetWebInitQuery, RequestResult<WebInitData>>
        {
            public readonly IMemoryCache _memoryCache;
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;
            private readonly ICountryService _countryService;
            private readonly IFileService _fileService;
            private readonly IBaseItemService _baseItemService;
            private readonly IBrandService _brandService;
            private readonly ICategoryService _categoryService;
            private readonly IProductService _productService;
            private readonly IProductFavoriteService _productFavoriteService;
            private readonly ICurrencyRateService _currencyRateService;

            public GetWebInitQueryHandler(IMemoryCache memoryCache, UserManager<AppUser> userManager, IMapper mapper, ICountryService countryService, IFileService fileService, IBaseItemService baseItemService, IBrandService brandService, ICategoryService categoryService, IProductService productService, IProductFavoriteService productFavoriteService, ICurrencyRateService currencyRateService)
            {
                _memoryCache = memoryCache;
                _userManager = userManager;
                _mapper = mapper;
                _countryService = countryService;
                _fileService = fileService;
                _baseItemService = baseItemService;
                _brandService = brandService;
                _categoryService = categoryService;
                _productService = productService;
                _productFavoriteService = productFavoriteService;
                _currencyRateService = currencyRateService;
            }

            public async Task<RequestResult<WebInitData>> Handle(GetWebInitQuery query, CancellationToken cancellationToken)
            {
                var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";
                var currencyRate = _currencyRateService.GetCurrencyRate(query.Currency);

                var user = string.IsNullOrEmpty(query.UserId) ? null : _userManager.Users.FirstOrDefault(x => x.Id == query.UserId);

                var cacheKey = $"GetWebInitCache_{lang}_{query.Currency}";
                var r = await _memoryCache.GetValueFromCache(cacheKey, async () =>
                {
                    var init = new WebInitData
                    {
                        Categories = new List<CategoryVM>(),
                    };

                    //Country
                    var clst = _countryService.Queryable().AsQueryable();
                    clst = query.Role != "Admin" ? clst.Where(x => x.IsActive) : clst;
                    var items = await clst.To<CountryVM>(_mapper).ToListAsync();
                    items.ForEach(c =>
                    {
                        c.Image = _fileService.GetFileUrl(c.Image);
                    });
                    init.Countries = items;

                    //Base Items
                    var lst = _baseItemService.Queryable().AsNoTracking()
                                .Where(x => x.IsHomeOrCategory == 0)
                                .OrderBy(x => x.OrderIndex).ToArray();
                    var bItems = new List<BaseItemVM>();

                    for (int i = 0; i < lst.Length; i++)
                    {
                        var item = lst[i];
                        if (item.BaseItemType == BaseItemTypes.SingleBanner &&
                            lst.ElementAtOrDefault(i + 1)?.BaseItemType == BaseItemTypes.SingleBanner)
                        {
                            var b1 = BaseItemVM.GenerateFromBaseItem(item);
                            var b2 = BaseItemVM.GenerateFromBaseItem(lst[i + 1]);
                            var bi = new BaseItemVM
                            {
                                BaseItemType = BaseItemTypes.MultipleBanners,
                                MultipleBanners = new BaseItemMultipleBanners { }
                            };

                            if (lst.ElementAtOrDefault(i + 2)?.BaseItemType == BaseItemTypes.SingleBanner)
                            {
                                var b3 = BaseItemVM.GenerateFromBaseItem(lst[i + 2]);
                                bi.MultipleBanners.Banners = new SingleBanner[] { b1.SingleBanner, b2.SingleBanner, b3.SingleBanner, };
                                i += 2;
                            }
                            else
                            {
                                bi.MultipleBanners.Banners = new SingleBanner[] { b1.SingleBanner, b2.SingleBanner, };
                                i++;
                            }
                            bItems.Add(bi);
                            continue;
                        }
                        var x = BaseItemVM.GenerateFromBaseItem(item);
                        BaseItemCleaner.CleanItems(x, _mapper, _brandService, _categoryService, _productService, query.lang, query.CurrencyRate);
                        bItems.Add(x);
                    }
                    init.BaseItems = bItems;

                    //Categories
                    var cats = await _categoryService.Queryable().AsNoTracking()
                                .Where(x => x.ParentId == null && x.IsActive)
                                .OrderBy(x => x.OrderIndex)
                                .ToArrayAsync();
                    int parentIndex = 0;
                    foreach (var cat in cats)
                    {
                        cat.OrderIndex = parentIndex++;
                        cat.Children = await _categoryService.Queryable().Where(x => x.ParentId == cat.Id && x.IsActive).OrderBy(x => x.OrderIndex).ToArrayAsync();
                        int subIndex = 0;
                        foreach (var subCat in cat.Children)
                        {
                            subCat.OrderIndex = subIndex++;
                            subCat.Children = await _categoryService.Queryable().Where(x => x.ParentId == subCat.Id && x.IsActive).OrderBy(x => x.OrderIndex).ToListAsync();

                            int subsubIndex = 0;
                            foreach (var subsub in subCat.Children)
                                subsub.OrderIndex = subsubIndex++;
                        }
                        init.Categories.Add(_mapper.Map<CategoryVM>(cat));
                    }
                    return init;
                }, DateTime.Now.AddSeconds(10));

                //Calculate IsFavorite
                if (query.Role == "Customer")
                {
                    foreach (var item in r.BaseItems)
                    {
                        if (item.BaseItemType == BaseItemTypes.ProductsSet)
                        {
                            var productsIds = item.Products.Select(x => x.Id).ToArray();
                            var exitedProductIds = _productFavoriteService.Queryable().Where(x => productsIds.Contains(x.ProductId) && x.AppUserId == query.UserId).Select(x => x.ProductId);
                            foreach (var p in item.Products)
                                p.IsFavorite = exitedProductIds.Contains(p.Id);
                        }
                    }
                }

                return new RequestResult<WebInitData>(r);
            }
        }
    }
}
