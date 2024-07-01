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
                if (await _countryService.Queryable().AnyAsync() == false)
                {
                    _countryService.Insert(new()
                    {
                        AverageDeliveryDays = 0,
                        Code = "TR",
                        CurrencyCode = "TL",
                        DeliveryLimitAmount = 0,
                        DeliveryLimitFixedAmount = 0,
                        DeliveryLimitPercent = 0,
                        IsActive = true,
                        IsDelivery = true,
                        NameAr = "بلد الجحيش",
                        NameEn = "Turkey",
                        NameTr = "Türkye",
                        Cities = [new() { NameAr = "عنتاب", NameEn = "Antep", NameTr = "Antep" }],
                        BannerImage = "",
                        Image = "",
                        OnDoorPayment = true,
                        PhoneCode = 1,
                        PrivacyPolicyText = ""
                    });
                    await _unitOfWork.SaveChangesAsync();
                }
                var lst = _countryService.Queryable();
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
