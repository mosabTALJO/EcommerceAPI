using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Inits.Queries
{
    public class GetAdminInitQuery : IRequest<RequestResult<InitDataAdminModel>>
    {
        public class GetAdminInitQueryHandler : IRequestHandler<GetAdminInitQuery, RequestResult<InitDataAdminModel>>
        {
            private readonly ICategoryService _categoryService;
            private readonly IBrandService _brandService;
            private readonly IMapper _mapper;
            public GetAdminInitQueryHandler(ICategoryService categoryService, IBrandService brandService, IMapper mapper)
            {
                _categoryService = categoryService;
                _brandService = brandService;
                _mapper = mapper;
            }

            public async Task<RequestResult<InitDataAdminModel>> Handle(GetAdminInitQuery request, CancellationToken cancellationToken)
            {
                var lst = await _categoryService.GetLiteCategories();
                var init = new InitDataAdminModel
                {
                    Brands = await _brandService.Queryable().Take(10).To<BrandVM>(_mapper).ToArrayAsync(),
                    Categories = lst.Select(x => new CategoryLite(x.Id, x.NameEn, x.ParentId)).ToList(),
                };
                return new RequestResult<InitDataAdminModel>(init);
            }
        }
    }
}
