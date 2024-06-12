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
    public class UpdateCountryCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string NameTr { get; set; }
        public double DeliveryLimitAmount { get; set; }
        public double DeliveryLimitFixedAmount { get; set; }
        public double DeliveryLimitPercent { get; set; }
        public int AverageDeliveryDays { get; set; }
        public string Code { get; set; }
        public string CurrencyCode { get; set; }
        public string BannerImage { get; set; }
        public int PhoneCode { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public bool OnDoorPayment { get; set; }
        //public bool MyFatoorahPayment { get; set; }
        public string PrivacyPolicyText { get; set; }
        public ICollection<CityVM> Cities { get; set; }

        public class PutCountryCommandHandler : IRequestHandler<UpdateCountryCommand, RequestResult<bool>>
        {
            private readonly ICountryService _countryService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            public PutCountryCommandHandler(ICountryService countryService, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _countryService = countryService;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<RequestResult<bool>> Handle(UpdateCountryCommand command, CancellationToken cancellationToken)
            {
                if (await _countryService.Queryable().AsNoTracking().AsNoTracking().FirstOrDefaultAsync(x => x.Id == command.Id) is Country dbCountry)
                {
                    dbCountry.IsActive = command.IsActive;
                    dbCountry.BannerImage = command.BannerImage;
                    dbCountry.DeliveryLimitAmount = command.DeliveryLimitAmount;
                    dbCountry.DeliveryLimitPercent = command.DeliveryLimitPercent;
                    dbCountry.AverageDeliveryDays = command.AverageDeliveryDays;
                    dbCountry.DeliveryLimitFixedAmount = command.DeliveryLimitFixedAmount;
                    dbCountry.OnDoorPayment = command.OnDoorPayment;
                    //dbCountry.MyFatoorahPayment = command.MyFatoorahPayment;
                    dbCountry.PrivacyPolicyText = command.PrivacyPolicyText;

                    _countryService.UpdateCountryData(dbCountry, command.Cities.Select(x => _mapper.Map<City>(x)).ToArray());

                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return new RequestResult<bool>(true);
                }
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
