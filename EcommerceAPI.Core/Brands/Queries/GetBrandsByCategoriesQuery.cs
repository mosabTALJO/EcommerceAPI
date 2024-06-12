using EcommerceAPI.Data;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Brands.Queries
{
    public class GetBrandsByCategoriesQuery : IRequest<RequestResult<BrandVM[]>>
    {
        public int[] CategoriesId { get; set; }
        public class GetBrandsByCategoriesQueryHandler : IRequestHandler<GetBrandsByCategoriesQuery, RequestResult<BrandVM[]>>
        {
            private readonly MyDbContext dbContext;
            public GetBrandsByCategoriesQueryHandler(MyDbContext dbContext)
            {
                this.dbContext = dbContext;
            }

            public async Task<RequestResult<BrandVM[]>> Handle(GetBrandsByCategoriesQuery query, CancellationToken cancellationToken)
            {
                var brands = await (from x in dbContext.Products
                    .Where(x => query.CategoriesId.Contains(x.CategoryId.Value))
                    .GroupBy(x => x.BrandId).Select(x => new { Id = x.Key.Value, ProductsCount = x.Count() })
                                    from b in dbContext.Brands
                                    where b.Id == x.Id
                                    select new BrandVM
                                    {
                                        Id = b.Id,
                                        NameEn = b.NameEn,
                                        Image = b.Image,
                                        Icon = string.IsNullOrEmpty(b.Icon) ? "NoImage" : b.Icon,
                                        ProductsCount = x.ProductsCount,
                                    })
                                    .Where(x=> x.ProductsCount > 100)
                                    .OrderByDescending(x => x.ProductsCount)
                                    .ToArrayAsync();
                return new RequestResult<BrandVM[]>(brands);
            }
        }
    }
}
