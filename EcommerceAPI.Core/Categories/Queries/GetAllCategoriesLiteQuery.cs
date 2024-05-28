using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;

namespace EcommerceAPI.Core.Categories.Queries
{
    public class GetAllCategoriesLiteQuery : IRequest<RequestResult<CategoryLite[]>>
    {
        public class GetAllCategoriesLiteQueryHandler : IRequestHandler<GetAllCategoriesLiteQuery, RequestResult<CategoryLite[]>>
        {
            private readonly ICategoryService _categoryService;
            public GetAllCategoriesLiteQueryHandler(ICategoryService categoryService)
            {
                _categoryService = categoryService;
            }
            public async Task<RequestResult<CategoryLite[]>> Handle(GetAllCategoriesLiteQuery query, CancellationToken cancellationToken)
            {
                var lst = await _categoryService.GetLiteCategories();
                return new RequestResult<CategoryLite[]>(lst.Select(x => new CategoryLite(x.Id, x.NameEn, x.ParentId)).ToArray());
            }
        }
    }
}
