using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Countries.Queries
{
    public class GetAllCountriesQuery : IRequest<RequestResult<CountryVM[]>>
    {
        public string Role { get; set; }
        public class GetAllContriesQueryHandler : IRequestHandler<GetAllCountriesQuery, RequestResult<CountryVM[]>>
        {
            private readonly ICountryService _countryService;
            private readonly IMapper _mapper;
            private readonly IFileService _fileService;
            public GetAllContriesQueryHandler(ICountryService countryService, IMapper mapper, IFileService fileService)
            {
                _countryService = countryService;
                _mapper = mapper;
                _fileService = fileService;
            }

            public async Task<RequestResult<CountryVM[]>> Handle(GetAllCountriesQuery query, CancellationToken cancellationToken)
            {
                var lst = _countryService.Queryable().Include(c => c.GiftPriceRanges).AsQueryable();
                lst = query.Role != "Admin" ? lst.Where(x => x.IsActive) : lst;

                var items = await lst.To<CountryVM>(_mapper).ToArrayAsync();

                foreach (var item in items)
                {
                    item.Image = _fileService.GetFileUrl(item.Image);
                }

                return new RequestResult<CountryVM[]>(items);
            }
        }
    }
}
