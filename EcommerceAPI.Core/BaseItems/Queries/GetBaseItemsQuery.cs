using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.BaseItems.Queries
{
    public class GetBaseItemsQuery : IRequest<RequestResult<ListResponse<BaseItemVM>>>
    {
        public int IsHomeOrCategory { get; set; }
        public string UserId { get; set; }
        public string lang { get; set; }
        public string Role { get; set; }
        public double CurrencyRate { get; set; }
        public bool isMobile { get; set; }//ToBeRemoved
        public class GetBaseItemsQueryHandler : IRequestHandler<GetBaseItemsQuery, RequestResult<ListResponse<BaseItemVM>>>
        {
            private readonly IBrandService _brandService;
            private readonly ICategoryService _categoryService;
            private readonly IProductService _productService;
            private readonly IBaseItemService _baseItemService;
            private readonly IMapper _mapper;
            private readonly IProductFavoriteService _productFavoriteService;
            public GetBaseItemsQueryHandler(IProductService productService, ICategoryService categoryService, IBrandService brandService, IBaseItemService baseItemService, IMapper mapper, IProductFavoriteService productFavoriteService)
            {
                _productService = productService;
                _categoryService = categoryService;
                _brandService = brandService;
                _baseItemService = baseItemService;
                _mapper = mapper;
                _productFavoriteService = productFavoriteService;
            }

            public async Task<RequestResult<ListResponse<BaseItemVM>>> Handle(GetBaseItemsQuery query, CancellationToken cancellationToken)
            {
                var lst = _baseItemService.Queryable().AsNoTracking()
               .Where(x => x.IsHomeOrCategory == query.IsHomeOrCategory)
               .OrderBy(x => x.OrderIndex).ToArray();
                var items = new List<BaseItemVM>();

                foreach (var item in lst)
                {
                    var x = BaseItemVM.GenerateFromBaseItem(item);
                    BaseItemCleaner.CleanItems(x, _mapper, _brandService, _categoryService, _productService, query.lang, query.CurrencyRate);

                    // calculate IsFavorite
                    if (query.Role == "Customer" && x.BaseItemType == BaseItemTypes.ProductsSet)
                    {
                        var productsIds = x.Products.Select(x => x.Id).ToArray();
                        {
                            var exitedProductIds = _productFavoriteService.Queryable().Where(x => productsIds.Contains(x.ProductId) && x.AppUserId == query.UserId).Select(x => x.ProductId);
                            foreach (var p in x.Products)
                                p.IsFavorite = exitedProductIds.Contains(p.Id);
                        }
                    }

                    items.Add(x);
                }

                return new RequestResult<ListResponse<BaseItemVM>>(new ListResponse<BaseItemVM> { TotalRecords = items.Count, Items = items.ToArray() });
            }
        }
    }
}
