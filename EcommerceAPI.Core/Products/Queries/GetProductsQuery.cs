using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace EcommerceAPI.Core.Products.Queries
{
    public class GetProductsQuery : IRequest<RequestResult<ListResponse<ProductLiteVM>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; }
        public int? CategoryId { get; set; }
        public int? CollectionId { get; set; }
        public int? BrandId { get; set; }
        public string Search { get; set; }

        public string Brands { get; set; }
        public string Colors { get; set; }
        public string Gender { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public bool ShowHidden { get; set; }
        public string UserId { get; set; }
        public string Currency { get; set; }
        public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, RequestResult<ListResponse<ProductLiteVM>>>
        {
            private readonly IProductService _productService;
            private readonly ICategoryService _categoryService;
            private readonly IOrderCategoryBrandService _orderCategoryBrandService;
            private readonly ICollectionService _collectionService;
            private readonly IProductFavoriteService _productFavoriteService;
            private readonly ICurrencyRateService _currencyRateService;
            private readonly IMemoryCache _memoryCache;

            public GetProductsQueryHandler(ICollectionService collectionService, IOrderCategoryBrandService orderCategoryBrandService, ICategoryService categoryService, IProductService productService, IProductFavoriteService productFavoriteService, IMemoryCache memoryCache, ICurrencyRateService currencyRateService)
            {
                _collectionService = collectionService;
                _orderCategoryBrandService = orderCategoryBrandService;
                _categoryService = categoryService;
                _productService = productService;
                _productFavoriteService = productFavoriteService;
                _memoryCache = memoryCache;
                _currencyRateService = currencyRateService;
            }

            public async Task<RequestResult<ListResponse<ProductLiteVM>>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
            {
                query.PageNumber = query.PageNumber.PagingNumber();
                IQueryable<Product> lst = null;
                var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";
                var currencyRate = _currencyRateService.GetCurrencyRate(query.Currency);

                ListResponse<ProductLiteVM> result = null;
                //Filtering used
                if (!string.IsNullOrEmpty(query.Brands) || !string.IsNullOrEmpty(query.Colors) || !string.IsNullOrEmpty(query.Gender) || query.MinPrice.HasValue || query.MaxPrice.HasValue)
                {
                    if (query.CollectionId.HasValue)
                    {
                        lst = _collectionService.Queryable().AsNoTracking()
                            .Include(x => x.Items).ThenInclude(x => x.Product.Brand)
                            .Include(x => x.Items).ThenInclude(x => x.Product.Category)
                            .Include(x => x.Items).ThenInclude(x => x.Product.ProductVariants)
                            .Where(x => x.Id == query.CollectionId)
                            .SelectMany(x => x.Items.Select(x => x.Product))
                            .OrderByDescending(x => x.Id);
                    }
                    else
                        lst = _productService.Queryable().AsNoTracking()
                        .Include(x => x.Brand)
                        .Include(x => x.Category)
                        .Where(x => x.Brand.IsActive)
                        .Where(x => !x.DeletedAt.HasValue)
                        .Where(x => x.BrandId != null);

                    if (query.CategoryId.HasValue)
                    {
                        var allCats = await _categoryService.GetRelatedCategories(query.CategoryId.Value);
                        if (allCats.Count == 1)
                            lst = lst.Where(x => x.CategoryId != null && x.CategoryId == allCats[0]);
                        else
                            lst = lst.Where(x => x.CategoryId != null && allCats.Contains(x.CategoryId.Value));
                    }
                    if (query.BrandId.HasValue)
                    {
                        lst = lst.Where(x => x.BrandId == query.BrandId.Value);
                    }

                    if (!string.IsNullOrEmpty(query.Search))
                    {
                        if (int.TryParse(query.Search, out int idToSearch))
                            lst = lst.Where(x => x.Id == idToSearch || EF.Functions.Like(x.NameTr!.ToLower(), query.Search.ToLower() + "%"));
                        else
                            lst = lst.Where(x => EF.Functions.Like(x.NameTr!.ToLower(), query.Search.ToLower() + "%"));
                    }

                    //MIN PRICe
                    if (query.MinPrice.HasValue)
                    {
                        var min = query.MinPrice.Value;
                        lst = lst.Where(x =>
                            currencyRate *
                            (
                               x.Discount != null ? x.SalesPrice * (1 - x.Discount.Value / 100) :
                               x.Category.Discount != null ? x.SalesPrice * (1 - x.Category.Discount.Value / 100) :
                               x.SalesPrice
                            )
                            >= min);
                    }
                    //MAX PRICe
                    if (query.MaxPrice.HasValue)
                    {
                        var max = query.MaxPrice.Value;
                        lst = lst.Where(x =>
                             currencyRate *
                             (
                                x.Discount != null ? x.SalesPrice * (1 - x.Discount.Value / 100) :
                                x.Category.Discount != null ? x.SalesPrice * (1 - x.Category.Discount.Value / 100) :
                                x.SalesPrice
                             )
                             <= max);
                    }
                    //BRANDS
                    if (!string.IsNullOrEmpty(query.Brands))
                    {
                        List<int> dbBrands = new List<int>();
                        foreach (var bidText in query.Brands.Split(","))
                        {
                            if (int.TryParse(bidText, out int bid))
                                dbBrands.Add(bid);
                        }
                        lst = lst.Where(x => x.BrandId.HasValue && dbBrands.Contains(x.BrandId.Value));
                    }
                    //COLORS
                    if (!string.IsNullOrEmpty(query.Colors))
                    {
                        //var dbColors = query.Colors.Split(",");
                        //lst = lst.Where(x => dbColors.Contains(x.CompanyColor));
                    }
                    //Gender
                    if (!string.IsNullOrEmpty(query.Gender))
                    {
                        //List<int> dbGender = new List<int>();
                        //foreach (var genderText in query.Gender.Split(","))
                        //{
                        //    if (int.TryParse(genderText, out int g))
                        //        dbGender.Add(g);
                        //}
                        //lst = lst.Where(x => x.GenderId.HasValue && dbGender.Contains(x.GenderId.Value));
                    }

                    result = new ListResponse<ProductLiteVM>
                    {
                        Items = await lst.Skip(query.PageSize * query.PageNumber).Take(query.PageSize).Select(x => new ProductLiteVM
                        {
                            Id = x.Id,
                            Name = lang == "en" ? x.NameEn : x.NameAr,
                            BrandName = lang == "en" ? x.Brand.NameEn : lang == "ar" ? x.Brand.NameAr : x.Brand.NameTr,
                            SalesPrice = currencyRate == 0 ? x.SalesPrice : x.SalesPrice * currencyRate,
                            Discount = x.Discount != null ? x.Discount : x.Category.Discount != null ? x.Category.Discount : null,
                            MainImage = x.MainImage,
                            TotalRate = x.TotalRate,
                            CreatedAt = x.CreatedAt,
                        }).ToArrayAsync(),
                        TotalRecords = await lst.CountAsync()
                    };
                }
                else
                {
                    Func<Task<ListResponse<ProductLiteVM>>> action = async () =>
                    {
                        var result = new ListResponse<ProductLiteVM>();
                        if (query.CollectionId.HasValue)
                        {
                            lst = _collectionService.Queryable().AsNoTracking()
                                .Include(x => x.Items).ThenInclude(x => x.Product.Brand)
                                .Include(x => x.Items).ThenInclude(x => x.Product.Category)
                                .Include(x => x.Items).ThenInclude(x => x.Product.ProductVariants)
                                .Where(x => x.Id == query.CollectionId)
                                .SelectMany(x => x.Items.Select(x => x.Product))
                                .OrderByDescending(x => x.Id);

                            result.Items = await lst.Skip(query.PageSize * query.PageNumber).Take(query.PageSize).Select(x => new ProductLiteVM
                            {
                                Id = x.Id,
                                Name = lang == "en" ? x.NameEn : lang == "ar" ? x.NameAr : x.NameTr,
                                BrandName = lang == "en" ? x.Brand.NameEn : lang == "ar" ? x.Brand.NameAr : x.Brand.NameTr,
                                SalesPrice = currencyRate == 0 ? x.SalesPrice : x.SalesPrice * currencyRate,
                                Discount = x.Discount > 0 ? x.Discount : x.Category.Discount > 0 ? x.Category.Discount : null,
                                MainImage = x.MainImage,
                                TotalRate = x.TotalRate,
                                CreatedAt = x.CreatedAt,
                            }).ToArrayAsync();
                            result.TotalRecords = await lst.CountAsync();
                        }
                        else
                        {
                            lst = _productService.Queryable()
                                .Include(x => x.Brand)
                                .Include(x => x.Category)
                                .Where(x => x.Brand.IsActive)
                                .Where(x => !x.DeletedAt.HasValue)
                                .Where(x => x.BrandId != null)
                                .IgnoreQueryFilters()
                                .Where(x => x.IsActive != query.ShowHidden);

                            if (query.CategoryId.HasValue)
                            {
                                var allCats = await _categoryService.GetRelatedCategories(query.CategoryId.Value);
                                if (allCats.Count == 1)
                                    lst = lst.Where(x => x.CategoryId == allCats[0]);
                                else
                                    lst = lst.Where(x => x.CategoryId != null && allCats.Contains(x.CategoryId.Value));
                            }

                            if (!string.IsNullOrEmpty(query.Search))
                            {
                                if (int.TryParse(query.Search, out int idToSearch))
                                    lst = lst.Where(x => x.Id == idToSearch || EF.Functions.Like(x.NameTr!.ToLower(), query.Search.ToLower() + "%"));
                                else
                                    lst = lst.Where(x => EF.Functions.Like(x.NameTr!.ToLower(), query.Search.ToLower() + "%"));
                            }

                            //enable 5 products per brand if there is no other filter
                            if (query.CategoryId.HasValue && !query.BrandId.HasValue)
                            {
                                var lstNew = new List<ProductLiteVM>();
                                var lstBrands = lst
                                    .GroupBy(x => new { x.BrandId, x.Brand.OrderIndex })
                                    .Select(x => new BrandProductsHelper { BrandId = x.Key.BrandId, OrderIndex = x.Key.OrderIndex, ProductsCount = x.Count() })
                                    .OrderBy(x => x.OrderIndex)
                                    .ToList();

                                var orderedBrands = await _orderCategoryBrandService.Queryable()
                                    .Where(x => x.CategoryId == query.CategoryId.Value)
                                    .OrderBy(x => x.OrderIndex)
                                    .ToArrayAsync();

                                var allBrands = new List<BrandProductsHelper>();
                                foreach (var ob in orderedBrands)
                                {
                                    var bb = lstBrands.FirstOrDefault(x => x.BrandId == ob.BrandId);
                                    if (bb != null)
                                    {
                                        bb.OrderIndex = ob.OrderIndex;
                                        bb.ProductsToBeTaken = ob.Count;
                                        if (bb.ProductsToBeTaken > 0)
                                            allBrands.Add(bb);
                                        lstBrands.Remove(bb);
                                    }
                                }
                                int maxOrderIndex = orderedBrands.LastOrDefault()?.OrderIndex ?? 0;
                                foreach (var ob in lstBrands)
                                {
                                    ob.OrderIndex = maxOrderIndex + ob.OrderIndex;
                                    ob.ProductsToBeTaken = 5;
                                    allBrands.Add(ob);
                                }

                                var foundBrands = new List<BrandProductsHelper>();
                                int brandIndex = 0;
                                int pageNumberReached = -1;
                                int currentProductsReached = 0;
                                while (pageNumberReached != query.PageNumber && allBrands.Any()) // when changing page
                                {
                                    pageNumberReached++;
                                    var brndsToBeUpdated = allBrands.Where(x => x.TotalProductsToBeTaken > 0).ToList();
                                    brndsToBeUpdated.ForEach(x => { x.ProductsToSkip += x.TotalProductsToBeTaken; x.TotalProductsToBeTaken = 0; });
                                    currentProductsReached = 0;

                                    while (currentProductsReached < query.PageSize && allBrands.Any()) // for one page only
                                    {
                                        //if (pageNumberReached == 27 && query.PageNumber == 27 && query.PageSize == 30)
                                        //    throw new Exception(currentProductsReached + ", " + allBrands.Count + ", " + allBrands.FirstOrDefault()?.ProductsCount + ", BINDEX:" + brandIndex);
                                        var brand = allBrands.ElementAtOrDefault(brandIndex);
                                        var productsToTakeFromBrand = currentProductsReached <= query.PageSize - brand.ProductsToBeTaken ? brand.ProductsToBeTaken : query.PageSize - currentProductsReached;

                                        if (brand.ProductsCount >= productsToTakeFromBrand)
                                        {
                                            brand.TotalProductsToBeTaken += productsToTakeFromBrand;
                                            brand.ProductsCount -= productsToTakeFromBrand;
                                            currentProductsReached += productsToTakeFromBrand;
                                        }
                                        else
                                        {
                                            brand.TotalProductsToBeTaken += brand.ProductsCount;
                                            currentProductsReached += brand.ProductsCount;
                                            brand.ProductsCount = 0;
                                        }

                                        if (pageNumberReached == query.PageNumber && !foundBrands.Contains(brand))
                                            foundBrands.Add(brand);
                                        if (brand.ProductsCount == 0)
                                            allBrands.RemoveAt(brandIndex--);

                                        brandIndex = brandIndex == allBrands.Count - 1 ? 0 : brandIndex + 1;
                                    }
                                }
                                foreach (var b in foundBrands)
                                {
                                    lstNew.AddRange(lst.Where(x => x.BrandId == b.BrandId)
                                        .OrderByDescending(x => x.Id)
                                        .Skip(b.ProductsToSkip).Take(b.TotalProductsToBeTaken)
                                        .Select(x => new ProductLiteVM
                                        {
                                            Name = lang == "en" ? x.NameEn : lang == "ar" ? x.NameAr : x.NameTr,
                                            BrandName = lang == "en" ? x.Brand.NameEn : lang == "ar" ? x.Brand.NameAr : x.Brand.NameTr,
                                            SalesPrice = currencyRate == 0 ? x.SalesPrice : x.SalesPrice * currencyRate,
                                            Discount = x.Discount > 0 ? x.Discount : x.Category.Discount > 0 ? x.Category.Discount : null,
                                            Id = x.Id,
                                            MainImage = x.MainImage,
                                            TotalRate = x.TotalRate,
                                            CreatedAt = x.CreatedAt,
                                        }).ToArray());
                                }

                                result.TotalRecords = await lst.CountAsync();
                                result.Items = lstNew.ToArray();
                            }
                            else if (query.BrandId.HasValue)
                            {
                                lst = lst.Where(x => x.BrandId == query.BrandId.Value);

                                var lstNew = new List<ProductLiteVM>();
                                var allCategories = lst
                                    .GroupBy(x => new { x.CategoryId, x.Category.OrderIndex })
                                    .Select(x => new CategoryProductsHelper { CategoryId = x.Key.CategoryId, OrderIndex = x.Key.OrderIndex, ProductsCount = x.Count() })
                                    .OrderBy(x => x.OrderIndex)
                                    .ToList();

                                var foundBrands = new List<CategoryProductsHelper>();
                                int brandIndex = 0;
                                int pageNumberReached = -1;
                                int currentProductsReached = 0;

                                while (pageNumberReached != query.PageNumber && allCategories.Any()) // when changing page
                                {
                                    pageNumberReached++;
                                    var brndsToBeUpdated = allCategories.Where(x => x.ProductsToBeTaken > 0).ToList();
                                    brndsToBeUpdated.ForEach(x => { x.ProductsToSkip += x.ProductsToBeTaken; x.ProductsToBeTaken = 0; });
                                    currentProductsReached = 0;

                                    while (currentProductsReached < 20 && allCategories.Any()) // for one page only
                                    {
                                        var productsToTakeFromBrand = currentProductsReached <= 15 ? 5 : 20 - currentProductsReached;

                                        var category = allCategories.ElementAtOrDefault(brandIndex);
                                        if (category.ProductsCount >= productsToTakeFromBrand)
                                        {
                                            category.ProductsToBeTaken += productsToTakeFromBrand;
                                            category.ProductsCount -= productsToTakeFromBrand;
                                            currentProductsReached += productsToTakeFromBrand;
                                        }
                                        else
                                        {
                                            category.ProductsToBeTaken += category.ProductsCount;
                                            currentProductsReached += category.ProductsCount;
                                            category.ProductsCount = 0;
                                        }

                                        if (pageNumberReached == query.PageNumber && !foundBrands.Contains(category))
                                            foundBrands.Add(category);
                                        if (category.ProductsCount == 0)
                                            allCategories.RemoveAt(brandIndex--);

                                        brandIndex = brandIndex == allCategories.Count - 1 ? 0 : brandIndex + 1;
                                    }
                                }
                                foreach (var c in foundBrands)
                                {
                                    lstNew.AddRange(lst.Where(x => x.CategoryId == c.CategoryId)
                                        .OrderByDescending(x => x.Id)
                                        .Skip(c.ProductsToSkip).Take(c.ProductsToBeTaken)
                                        .Select(x => new ProductLiteVM
                                        {
                                            Name = lang == "en" ? x.NameEn : lang == "ar" ? x.NameAr : x.NameTr,
                                            BrandName = lang == "en" ? x.Brand.NameEn : lang == "ar" ? x.Brand.NameAr : x.Brand.NameTr,
                                            SalesPrice = currencyRate == 0 ? x.SalesPrice : x.SalesPrice * currencyRate,
                                            Discount = x.Discount > 0 ? x.Discount : x.Category.Discount > 0 ? x.Category.Discount : null,
                                            Id = x.Id,
                                            MainImage = x.MainImage,
                                            TotalRate = x.TotalRate,
                                            CreatedAt = x.CreatedAt,
                                        }).ToArray());
                                }

                                result.TotalRecords = await lst.CountAsync();
                                result.Items = lstNew.ToArray();
                            }
                            else
                            {
                                if (query.BrandId.HasValue)
                                    lst = lst.Where(x => x.BrandId == query.BrandId.Value);

                                var lstResult = lst.OrderByDescending(x => x.Id).Select(x => new ProductLiteVM
                                {
                                    Name = lang == "en" ? x.NameEn : lang == "ar" ? x.NameAr : x.NameTr,
                                    BrandName = lang == "en" ? x.Brand.NameEn : lang == "ar" ? x.Brand.NameAr : x.Brand.NameTr,
                                    SalesPrice = currencyRate == 0 ? x.SalesPrice : x.SalesPrice * currencyRate,
                                    Discount = x.Discount > 0 ? x.Discount : x.Category.Discount > 0 ? x.Category.Discount : null,
                                    Id = x.Id,
                                    MainImage = x.MainImage,
                                    TotalRate = x.TotalRate,
                                    CreatedAt = x.CreatedAt,
                                });

                                result.TotalRecords = await lst.CountAsync(); //(!query.BrandId.HasValue && !query.CategoryId.HasValue && string.IsNullOrEmpty(query.Search)) ? 1000000 : await lst.CountAsync();
                                result.Items = await lstResult.Skip(query.PageSize * query.PageNumber).Take(query.PageSize).ToArrayAsync();
                            }
                        }

                        return result;
                    };

                    var cacheKey = $"ProductCache-{query.PageNumber}_{query.PageSize}_{query.CategoryId}_{query.CollectionId}_{query.BrandId}_{query.Search}_{lang}_{query.Currency}";
                    //ignore cache for the last level of Categories
                    var ignoreCache = query.CategoryId.HasValue && !_categoryService.Queryable().Any(x => x.ParentId == query.CategoryId.Value);

                    result = ignoreCache ? await action.Invoke() : await _memoryCache.GetValueFromCache(cacheKey, action);
                }

                //calcualte IsFavorite
                if (!string.IsNullOrEmpty(query.UserId))
                {
                    var productIds = result.Items.Select(x => x.Id);
                    var exitedProductIds = _productFavoriteService.Queryable().Where(x => productIds.Contains(x.ProductId) && x.AppUserId == query.UserId).Select(x => x.ProductId);
                    foreach (var p in result.Items)
                        p.IsFavorite = exitedProductIds.Contains(p.Id);
                }

                return new RequestResult<ListResponse<ProductLiteVM>>(result);
            }
        }
    }
    class BrandProductsHelper
    {
        public int? BrandId { get; set; }
        public int? OrderIndex { get; set; }
        public int ProductsCount { get; set; }
        public int ProductsToSkip { get; set; }
        public int TotalProductsToBeTaken { get; set; }
        public int ProductsToBeTaken { get; set; }
    }
    class CategoryProductsHelper
    {
        public int? CategoryId { get; set; }
        public int? OrderIndex { get; set; }
        public int ProductsCount { get; set; }
        public int ProductsToSkip { get; set; }
        public int ProductsToBeTaken { get; set; }
    }
}
