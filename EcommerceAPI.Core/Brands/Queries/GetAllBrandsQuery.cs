using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Brands.Queries
{
    public class GetAllBrandsQuery : IRequest<RequestResult<ListResponse<BrandVM>>>
    {
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public int? CategoryId { get; set; } = null;
        public string? Search { get; set; } = null;
        public string SortField { get; set; } = "ProductsCount";
        public string SortOrder { get; set; } = "Asc";
        public bool IncludeDeleted { get; set; } = false;
        public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, RequestResult<ListResponse<BrandVM>>>
        {
            private readonly IBrandService _brandService;
            private readonly IMapper _mapper;
            private readonly IProductService _productService;
            public GetAllBrandsQueryHandler(IBrandService brandService, IMapper mapper, IProductService productService)
            {
                _brandService = brandService;
                _mapper = mapper;
                _productService = productService;
            }
            public async Task<RequestResult<ListResponse<BrandVM>>> Handle(GetAllBrandsQuery query, CancellationToken cancellationToken)
            {
                var lst = _brandService.Queryable().Where(x => query.IncludeDeleted ? true : x.IsActive);

                if (!string.IsNullOrEmpty(query.Search))
                    lst = lst.Where(x => x.NameAr.Contains(query.Search) || x.NameEn.Contains(query.Search) || x.NameTr.Contains(query.Search));
                if (query.CategoryId > 0)
                {
                    var brandIds = _brandService.GetBrandIdsByCategoryId(query.CategoryId.Value);
                    lst = lst.Where(x => brandIds.Contains(x.Id));
                }

                lst = query.SortOrder?.ToLower() == "asc" ? lst.OrderBy(query.SortField) : lst.OrderByDescending(query.SortField);

                ListResponse<BrandVM> result = new ListResponse<BrandVM>
                {
                    TotalRecords = await lst.CountAsync(),
                    Items = await lst.Skip(query.PageSize * query.PageNumber).Take(query.PageSize)
                        .To<BrandVM>(_mapper).ToArrayAsync()
                };

                foreach (var b in result.Items)
                    b.ProductsCount = _productService.Queryable().Where(x => x.BrandId == b.Id).Count();
                return new RequestResult<ListResponse<BrandVM>>(result);
            }
        }
    }
}
