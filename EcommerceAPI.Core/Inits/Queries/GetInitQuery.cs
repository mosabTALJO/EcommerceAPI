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
using Microsoft.Extensions.Primitives;
using System.Globalization;

namespace EcommerceAPI.Core.Inits.Queries
{
    public class GetInitQuery : IRequest<RequestResult<InitData>>
    {
        public string UserId { get; set; }
        public string Role { get; set; }
        public StringValues Currency { get; set; }
        public class GetInitQueryHandler : IRequestHandler<GetInitQuery, RequestResult<InitData>>
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

            public GetInitQueryHandler(IMemoryCache memoryCache, UserManager<AppUser> userManager, IMapper mapper, ICountryService countryService, IFileService fileService, IBaseItemService baseItemService, IBrandService brandService, ICategoryService categoryService, IProductService productService, IProductFavoriteService productFavoriteService, ICurrencyRateService currencyRateService)
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

            public async Task<RequestResult<InitData>> Handle(GetInitQuery query, CancellationToken cancellationToken)
            {
                var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";
                var currencyRate = _currencyRateService.GetCurrencyRate(query.Currency);

                var user = string.IsNullOrEmpty(query.UserId) ? null : _userManager.Users.FirstOrDefault(x => x.Id == query.UserId);

                var cacheKey = $"GetInitCache_{lang}_{query.Currency}";
                var r = await _memoryCache.GetValueFromCache(cacheKey, async () =>
                {
                    var init = new InitData
                    {
                        Categories = new List<CategoryVM>(),
                        WomenCategoryIds = new List<int>(),
                        FirebaseTopics = new List<string> { "all", "All" },
                        User = user == null ? null : new UserVM
                        {
                            DisplayName = user.DisplayName,
                            Id = user.Id,
                            IsActive = user.IsActive,
                            PhoneNumber = user.PhoneNumber,
                            Email = user.Email,
                            UserType = user.UserType,
                            Country = user.Country,
                        }
                    };

                    //Country
                    var lstCountry = await _countryService.Queryable().Where(x => x.IsActive)
                        .To<CountryVM>(_mapper).ToListAsync();
                    lstCountry.ForEach(c => c.Image = _fileService.GetFileUrl(c.Image));
                    init.Countries = lstCountry;

                    //Base Items
                    var lst = _baseItemService.Queryable().AsNoTracking().Where(x => x.IsHomeOrCategory == 0)
                                   .OrderBy(x => x.OrderIndex).ToArray();
                    var items = new List<BaseItemVM>();
                    foreach (var item in lst)
                    {
                        var x = BaseItemVM.GenerateFromBaseItem(item);
                        BaseItemCleaner.CleanItems(x, _mapper, _brandService, _categoryService, _productService, lang, currencyRate);
                        items.Add(x);
                    }
                    init.BaseItems = items;

                    //Categories
                    var cats = await _categoryService.Queryable().AsNoTracking()
                                .Where(x => x.ParentId == null && x.IsActive)
                                .OrderBy(x => x.OrderIndex)
                                .ToArrayAsync();
                    int parentIndex = 0;
                    foreach (var cat in cats)
                    {
                        cat.OrderIndex = parentIndex++;

                        bool isWomen = cat.NameEn.ToLower() == "woman" || cat.NameEn.ToLower() == "women";
                        if (isWomen)
                            init.WomenCategoryIds.Add(cat.Id);

                        cat.Children = await _categoryService.Queryable().Where(x => x.ParentId == cat.Id && x.IsActive).OrderBy(x => x.OrderIndex).ToArrayAsync();
                        int subIndex = 0;
                        foreach (var subCat in cat.Children)
                        {
                            subCat.OrderIndex = subIndex++;
                            subCat.Children = await _categoryService.Queryable().Where(x => x.ParentId == subCat.Id && x.IsActive).OrderBy(x => x.OrderIndex).ToListAsync();

                            int subsubIndex = 0;
                            foreach (var subsub in subCat.Children)
                                subsub.OrderIndex = subsubIndex++;

                            if (isWomen)
                            {
                                init.WomenCategoryIds.Add(subCat.Id);
                                init.WomenCategoryIds.AddRange(subCat.Children.Select(x => x.Id));
                            }
                        }
                        init.Categories.Add(_mapper.Map<CategoryVM>(cat));
                    }

                    return init;
                }, DateTime.UtcNow.AddSeconds(10));

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

                return new RequestResult<InitData>(r);
            }
        }
    }
}
