using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Coupons.Queries
{
    public class GetAllCouponsQuery : IRequest<RequestResult<ListResponse<CouponVM>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; } = null;
        public string SortField { get; set; } = "Id";
        public string SortOrder { get; set; } = "Asc";
        public class GetAllCouponsQueryHandler : IRequestHandler<GetAllCouponsQuery, RequestResult<ListResponse<CouponVM>>>
        {
            private readonly ICouponService _couponService;
            private readonly IProductService _productService;
            private readonly IMapper _mapper;
            private readonly ICategoryService _categoryService;
            private readonly IBrandService _brandService;
            public GetAllCouponsQueryHandler(ICouponService couponService, IProductService productService, IMapper mapper, ICategoryService categoryService, IBrandService brandService)
            {
                _couponService = couponService;
                _productService = productService;
                _mapper = mapper;
                _categoryService = categoryService;
                _brandService = brandService;
            }

            public async Task<RequestResult<ListResponse<CouponVM>>> Handle(GetAllCouponsQuery query, CancellationToken cancellationToken)
            {
                var lst = _couponService.Queryable().Where(x => !x.DeletedAt.HasValue);
                if (!string.IsNullOrEmpty(query.Search))
                    lst = lst.Where(x => x.NameAr.Contains(query.Search) || x.NameEn.Contains(query.Search) || x.NameTr.Contains(query.Search));
                lst = query.SortOrder?.ToLower() == "asc" ? lst.OrderBy(query.SortField) : lst.OrderByDescending(query.SortField);

                ListResponse<CouponVM> result = new()
                {
                    TotalRecords = await lst.CountAsync(),
                    Items = await lst.Skip(query.PageSize * query.PageNumber).Take(query.PageSize)
                    .ProjectTo<CouponVM>(_mapper.ConfigurationProvider).ToArrayAsync()
                };

                foreach (var r in result.Items)
                    r.TypeName = r.Type == CouponType.Category ? await _categoryService.Queryable().Where(x => x.Id == r.TypeId).Select(x => x.NameEn).FirstOrDefaultAsync() :
                        r.Type == CouponType.Brand ? await _brandService.Queryable().Where(x => x.Id == r.TypeId).Select(x => x.NameEn).FirstOrDefaultAsync() :
                        r.Type == CouponType.Product ? await _productService.Queryable().Where(x => x.Id == r.TypeId).Select(x => x.NameEn).FirstOrDefaultAsync() : "";
                return new RequestResult<ListResponse<CouponVM>>(result);
            }
        }
    }
}
