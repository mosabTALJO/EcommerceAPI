using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Categories.Queries
{
    public class GetCategoryParentsQuery : IRequest<RequestResult<CategoryLite[]>>
    {
        public class GetCategoryParentsQueryHandler : IRequestHandler<GetCategoryParentsQuery, RequestResult<CategoryLite[]>>
        {
            private readonly ICategoryService _categoryService;
            public GetCategoryParentsQueryHandler(ICategoryService categoryService)
            {
                _categoryService = categoryService;
            }
            public async Task<RequestResult<CategoryLite[]>> Handle(GetCategoryParentsQuery query, CancellationToken cancellationToken)
            {
                var lst = await _categoryService.Queryable().Where(x => x.ParentId == null).Select(x => new CategoryLite { Id = x.Id, Name = x.NameEn, ParentId = x.ParentId }).ToListAsync();
                lst.Insert(0, new CategoryLite(0, "Home", null));
                return new RequestResult<CategoryLite[]>(lst.ToArray());
            }
        }
    }
}
