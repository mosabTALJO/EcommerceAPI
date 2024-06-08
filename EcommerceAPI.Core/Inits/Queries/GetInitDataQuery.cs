using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Inits.Queries
{
    public class GetInitDataQuery : IRequest<RequestResult<InitDataModel>>
    {
        public string UserId { get; set; }
        public string Role { get; set; }
        public double CurrencyRate { get; set; }
        public class GetInitDataQueryHandler : IRequestHandler<GetInitDataQuery, RequestResult<InitDataModel>>
        {
            private readonly ICategoryService _categoryService;
            private readonly IBrandService _brandService;
            private readonly IMapper _mapper;
            private readonly ICurrencyRateService _currencyService;
            private readonly UserManager<AppUser> _userManager;
            private readonly IProductService _productService;
            private readonly IProductFavoriteService _productFavoriteService;

            public GetInitDataQueryHandler(ICategoryService categoryService, IBrandService brandService, IMapper mapper, ICurrencyRateService currencyService, UserManager<AppUser> userManager, IProductService productService, IProductFavoriteService productFavoriteService)
            {
                _categoryService = categoryService;
                _brandService = brandService;
                _mapper = mapper;
                _currencyService = currencyService;
                _userManager = userManager;
                _productService = productService;
                _productFavoriteService = productFavoriteService;
            }
            public async Task<RequestResult<InitDataModel>> Handle(GetInitDataQuery query, CancellationToken cancellationToken)
            {
                var user = string.IsNullOrEmpty(query.UserId) ? null : _userManager.Users.FirstOrDefault(x => x.Id == query.UserId);

                var init = new InitDataModel
                {
                    Brands = [],
                    Colors = [],
                    Categories = [],
                    WomenCategoryIds = [],
                    CurrencyRates = await _currencyService.Queryable().To<CurrencyRateLiteVM>(_mapper).ToArrayAsync(),
                    Slides = [],
                    Sizes = [],
                    FirebaseTopics = ["all", "All"],
                    User = user == null ? null : new UserVM
                    {
                        DisplayName = user.DisplayName,
                        Id = user.Id,
                        IsActive = user.IsActive,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        UserType = user.UserType,
                        Country = user.Country,
                    }
                };

                var cats = await _categoryService.Queryable().AsNoTracking()
                            .Where(x => x.ParentId == null && x.IsActive)
                            .OrderBy(x => x.OrderIndex)
                            .ToArrayAsync();

                int parentIndex = 0;
                foreach (var cat in cats)
                {
                    cat.OrderIndex = parentIndex++;

                    bool isWomen = cat.NameEn.ToLower() == "woman" || cat.NameEn.ToLower() == "women";
                    if (isWomen)
                        init.WomenCategoryIds.Add(cat.Id);

                    cat.Children = await _categoryService.Queryable().Where(x => x.ParentId == cat.Id && x.IsActive).OrderBy(x => x.OrderIndex).ToArrayAsync();
                    int subIndex = 0;
                    foreach (var subCat in cat.Children)
                    {
                        subCat.OrderIndex = subIndex++;
                        subCat.Children = await _categoryService.Queryable().Where(x => x.ParentId == subCat.Id && x.IsActive).OrderBy(x => x.OrderIndex).ToListAsync();

                        int subsubIndex = 0;
                        foreach (var subsub in subCat.Children)
                            subsub.OrderIndex = subsubIndex++;

                        if (isWomen)
                        {
                            init.WomenCategoryIds.Add(subCat.Id);
                            init.WomenCategoryIds.AddRange(subCat.Children.Select(x => x.Id));
                        }
                    }
                    init.Categories.Add(_mapper.Map<CategoryVM>(cat));
                }

                return new RequestResult<InitDataModel>(init);
            }
        }
    }
}
