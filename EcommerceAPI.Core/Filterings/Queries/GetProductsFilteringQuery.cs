using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EcommerceAPI.Core.Filterings.Queries
{
    public class GetProductsFilteringQuery : IRequest<RequestResult<Filtering>>
    {
        public int? CategoryId { get; set; } = null;
        public int? CollectionId { get; set; } = null;
        public int? BrandId { get; set; } = null;
        public string Search { get; set; } = null;
        public class GetProductsFilteringQueryHandler : IRequestHandler<GetProductsFilteringQuery, RequestResult<Filtering>>
        {
            private readonly IProductService _productService;
            private readonly ICategoryService _categoryService;
            private readonly ICollectionService _collectionService;
            public GetProductsFilteringQueryHandler(IProductService productService, ICategoryService categoryService, ICollectionService collectionService)
            {
                _productService = productService;
                _categoryService = categoryService;
                _collectionService = collectionService;
            }

            public async Task<RequestResult<Filtering>> Handle(GetProductsFilteringQuery query, CancellationToken cancellationToken)
            {
                if (!query.CategoryId.HasValue && !query.BrandId.HasValue && !query.CollectionId.HasValue && string.IsNullOrWhiteSpace(query.Search))
                    return new RequestResult<Filtering>(new Filtering());

                var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";
                IQueryable<Product> lst = null;
                if (query.CollectionId.HasValue)
                {
                    lst = _collectionService.Queryable().AsNoTracking()
                            .Include(x => x.Items).ThenInclude(x => x.Product.Brand)
                            .Include(x => x.Items).ThenInclude(x => x.Product.Category)
                            .Where(x => x.Id == query.CollectionId)
                            .SelectMany(x => x.Items.Select(x => x.Product))
                            .Where(x => x.Brand.IsActive)
                            .Where(x => !x.DeletedAt.HasValue)
                            .Where(x => x.BrandId != null);
                }
                else
                {
                    lst = _productService.Queryable()
                     .Include(x => x.Brand)
                     .Where(x => x.Brand.IsActive)
                     .Where(x => !x.DeletedAt.HasValue)
                     .Where(x => x.BrandId != null);
                }

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
                        lst = lst.Where(x => x.Id == idToSearch || EF.Functions.Like(x.NameEn, query.Search + "%"));
                    else
                        lst = lst.Where(x => EF.Functions.Like(x.NameEn, query.Search + "%"));
                }

                return new RequestResult<Filtering>(new Filtering
                {
                    Brands = lst.Select(x => new FilterItem { Id = x.Brand.Id.ToString(), Name = lang == "ar" ? x.Brand.NameAr : x.Brand.NameEn }).Distinct().Take(200).ToArray(),
                    Colors = []// lst.Select(x => new FilterItem { Id = x.CompanyColor, Name = x.CompanyColor }).Distinct().Take(200).ToArray(),
                });
            }
        }
    }
    public class Filtering
    {
        public FilterItem[] Brands { get; set; }
        //public FilterItem[] Sizes { get; set; }
        public FilterItem[] Colors { get; set; }
    }
    public class FilterItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
