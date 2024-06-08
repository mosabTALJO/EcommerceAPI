using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Countries.Commands
{
    public class UpdateMultipleCountriesCommand : IRequest<RequestResult<bool>>
    {
        public UpdateCountryCommand[] Countries { get; set; }
        public class UpdateMultipleCountriesCommandHandler : IRequestHandler<UpdateMultipleCountriesCommand, RequestResult<bool>>
        {
            private readonly ICountryService _countryService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            public UpdateMultipleCountriesCommandHandler(ICountryService countryService, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _countryService = countryService;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }
            public async Task<RequestResult<bool>> Handle(UpdateMultipleCountriesCommand command, CancellationToken cancellationToken)
            {
                foreach (var country in command.Countries)
                {
                    var dbCountry = await _countryService.Queryable().AsNoTracking().FirstAsync(x => x.Id == country.Id);
                    dbCountry.IsActive = country.IsActive;
                    dbCountry.BannerImage = country.BannerImage;
                    dbCountry.DeliveryLimitAmount = country.DeliveryLimitAmount;
                    dbCountry.DeliveryLimitPercent = country.DeliveryLimitPercent;
                    dbCountry.AverageDeliveryDays = country.AverageDeliveryDays;
                    dbCountry.DeliveryLimitFixedAmount = country.DeliveryLimitFixedAmount;
                    dbCountry.OnDoorPayment = country.OnDoorPayment;
                    dbCountry.PrivacyPolicyText = country.PrivacyPolicyText;

                    _countryService.UpdateCountryData(dbCountry, country.Cities.Select(x => _mapper.Map<City>(x)).ToArray());
                }

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}