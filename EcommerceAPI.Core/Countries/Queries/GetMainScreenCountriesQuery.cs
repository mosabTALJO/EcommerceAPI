using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Countries.Queries
{
    public class GetMainScreenCountriesQuery : IRequest<RequestResult<List<CountryVM>>>
    {
        public class GetMainScreenCountriesQueryHandler : IRequestHandler<GetMainScreenCountriesQuery, RequestResult<List<CountryVM>>>
        {
            private readonly ICountryService _countryService;
            private readonly IFileService _fileService;
            private readonly IMapper _mapper;
            public GetMainScreenCountriesQueryHandler(ICountryService countryService, IFileService fileService, IMapper mapper)
            {
                _countryService = countryService;
                _fileService = fileService;
                _mapper = mapper;
            }
            public async Task<RequestResult<List<CountryVM>>> Handle(GetMainScreenCountriesQuery request, CancellationToken cancellationToken)
            {
                var lst = await _countryService.Queryable().Include(c => c.GiftPriceRanges).To<CountryVM>(_mapper).ToListAsync();
                lst.ForEach(c =>
                {
                    c.Image = _fileService.GetFileUrl(c.Image);
                });
                lst.Insert(0, new CountryVM { NameEn = "All Counties", Id = 0, });
                return new RequestResult<List<CountryVM>>(lst);
            }
        }
    }
}
