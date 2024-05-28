using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Categories.Queries
{
    public class GetCategoryByIdQuery : IRequest<RequestResult<CategoryVM>>
    {
        public int Id { get; set; }
        public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, RequestResult<CategoryVM>>
        {
            private readonly ICategoryService _categoryService;
            private readonly IMapper _mapper;
            public GetCategoryByIdQueryHandler(ICategoryService categoryService, IMapper mapper)
            {
                _categoryService = categoryService;
                _mapper = mapper;
            }
            public async Task<RequestResult<CategoryVM>> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
            {
                if (await _categoryService.Queryable().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == query.Id) is Category b)
                    return new RequestResult<CategoryVM>(_mapper.Map<CategoryVM>(b));
                return new RequestResult<CategoryVM>(null, ActionResultType.NotFound, "IdError");
            }
        }
    }
}
