using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace EcommerceAPI.Core.Countries.Queries
{
    public class GetAllCountriesQuery : IRequest<RequestResult<CountryVM[]>>
    {
        public string Role { get; set; }
        public class GetAllContriesQueryHandler : IRequestHandler<GetAllCountriesQuery, RequestResult<CountryVM[]>>
        {
            private readonly ICountryService _countryService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IFileService _fileService;
            public GetAllContriesQueryHandler(ICountryService countryService, IMapper mapper, IFileService fileService, IUnitOfWork unitOfWork)
            {
                _countryService = countryService;
                _mapper = mapper;
                _fileService = fileService;
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestResult<CountryVM[]>> Handle(GetAllCountriesQuery query, CancellationToken cancellationToken)
            {
                var lst = _countryService.Queryable();
                lst = query.Role != "Admin" ? lst.Where(x => x.IsActive) : lst;

                var items = await lst.To<CountryVM>(_mapper).ToArrayAsync();
                return new RequestResult<CountryVM[]>(items);
            }
        }
    }
}
