using AutoMapper;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Categories.Queries
{
    public class GetAllCategoriesQuery : IRequest<RequestResult<ListResponse<CategoryVM>>>
    {
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; } = null;
        public string SortField { get; set; } = "Id";
        public string SortOrder { get; set; } = "Asc";
        public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, RequestResult<ListResponse<CategoryVM>>>
        {
            private readonly IMapper _mapper;
            private readonly ICategoryService _categoryService;
            public GetAllCategoriesQueryHandler(IMapper mapper, ICategoryService categoryService)
            {
                _mapper = mapper;
                _categoryService = categoryService;
            }
            public async Task<RequestResult<ListResponse<CategoryVM>>> Handle(GetAllCategoriesQuery query, CancellationToken cancellationToken)
            {
                var lst = _categoryService.Queryable().IgnoreQueryFilters().Include(x => x.Parent).ThenInclude(x => x.Parent).AsNoTracking();
                if (!string.IsNullOrEmpty(query.Search))
                {
                    if (int.TryParse(query.Search, out int idSearch))
                        lst = lst.Where(x => x.Id == idSearch || x.NameAr.Contains(query.Search) || x.NameEn.Contains(query.Search) || x.NameTr.Contains(query.Search));
                    else
                        lst = lst.Where(x => x.NameAr.Contains(query.Search) || x.NameEn.Contains(query.Search) || x.NameTr.Contains(query.Search));
                }

                var lastList = await lst
                    .OrderByDescending(x => x.ParentId.HasValue)
                    .ThenByDescending(x => x.Parent.ParentId.HasValue)
                    .ThenBy(x => x.Parent.ParentId)
                    .ThenBy(x => x.ParentId)
                    .ThenBy(x => x.NameEn)
                    .Skip(query.PageSize * query.PageNumber).Take(query.PageSize).ToArrayAsync();

                foreach (var item in lastList)
                {
                    if (item.Parent != null)
                    {
                        if (item.Parent?.Parent != null)
                            item.HelperName = item.Parent.Parent.NameEn + " -> " + item.Parent.NameEn + " -> " + item.NameEn;
                        else
                            item.HelperName = item.Parent.NameEn + " -> " + item.NameEn;
                    }
                    else
                    {
                        item.HelperName = item.NameEn;
                    }
                }

                foreach (var item in lastList)
                    item.HelperName = item.Id + ": " + item.HelperName;

                var result = new ListResponse<CategoryVM>
                {
                    TotalRecords = await lst.CountAsync(),
                    //items = await lst.Skip(pageSize * pageNumber).Take(pageSize) .To<CategoryVM>(_mapper).ToArrayAsync()
                    Items = lastList.Select(x => _mapper.Map<CategoryVM>(x)).ToArray()
                };


                return new RequestResult<ListResponse<CategoryVM>>(result);
            }
        }
    }
}
