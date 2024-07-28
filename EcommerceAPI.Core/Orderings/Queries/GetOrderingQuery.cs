using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orderings.Queries
{
    public class GetOrderingQuery : IRequest<RequestResult<OrderCategoryBrandVM[]>>
    {
        public int CategoryId { get; set; }
        public string Search { get; set; }
        public class GetOrderingQueryHandler : IRequestHandler<GetOrderingQuery, RequestResult<OrderCategoryBrandVM[]>>
        {
            private readonly IOrderCategoryBrandService _orderCategoryBrandService;
            private readonly ICategoryService _categoryService;
            private readonly IMapper _mapper;
            private readonly MyDbContext dbContext;

            public GetOrderingQueryHandler(IOrderCategoryBrandService orderCategoryBrandService, ICategoryService categoryService, IMapper mapper, MyDbContext dbContext)
            {
                _orderCategoryBrandService = orderCategoryBrandService;
                _categoryService = categoryService;
                _mapper = mapper;
                this.dbContext = dbContext;
            }

            public async Task<RequestResult<OrderCategoryBrandVM[]>> Handle(GetOrderingQuery query, CancellationToken cancellationToken)
            {
                var lst = _orderCategoryBrandService.Queryable().AsNoTracking().Where(x => x.CategoryId == query.CategoryId)
                .OrderBy(x => x.OrderIndex).To<OrderCategoryBrandVM>(_mapper).ToList();

                var categoryIds = await _categoryService.GetRelatedCategories(query.CategoryId);

                var brands = await (from x in dbContext.Products
                                        .Where(x => x.IsActive && categoryIds.Contains(x.CategoryId.Value))
                                        .GroupBy(x => x.BrandId).Select(x => new { Id = x.Key.Value, ProductsCount = x.Count() })
                                    from b in dbContext.Brands
                                    where b.Id == x.Id && b.IsActive
                                    where string.IsNullOrEmpty(query.Search) || b.NameAr.ToLower().Contains(query.Search) || b.NameEn.ToLower().Contains(query.Search) || b.NameTr.ToLower().Contains(query.Search)
                                    select new BrandVM
                                    {
                                        Id = b.Id,
                                        NameEn = b.NameEn,
                                        Image = b.Image,
                                        Icon = b.Icon,
                                        ProductsCount = x.ProductsCount,
                                    })
                                    .OrderByDescending(x => x.ProductsCount)
                                    .ToArrayAsync();

                int orderIndex = lst.Count + 1;
                foreach (var b in brands)
                {
                    var item = lst.FirstOrDefault(x => x.BrandId == b.Id);
                    if (item is null)
                    {
                        item = new OrderCategoryBrandVM
                        {
                            BrandId = b.Id,
                            BrandName = b.NameEn,
                            BrandImage = b.Image,
                            Count = 5,
                            CategoryId = query.CategoryId,
                            OrderIndex = orderIndex++,
                        };
                        lst.Add(item);
                    }
                    item.ProductsCount = b.ProductsCount;
                    item.BrandName = b.NameEn;
                    item.BrandImage = b.Icon ?? "NoImage";
                }
                //clean the list to include the existed brands only
                for (int i = 0; i < lst.Count; i++)
                    if (!brands.Any(x => x.Id == lst[i].BrandId))
                        lst.RemoveAt(i--);

                return new RequestResult<OrderCategoryBrandVM[]>(lst.ToArray());
            }
        }
    }
}
