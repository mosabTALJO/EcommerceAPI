using AutoMapper;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Countries.Commands
{
    public class CreateCountryCommand : IRequest<RequestResult<bool>>
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string NameTr { get; set; } = string.Empty;
        public double DeliveryLimitAmount { get; set; }
        public double DeliveryLimitFixedAmount { get; set; }
        public double DeliveryLimitPercent { get; set; }
        public int AverageDeliveryDays { get; set; }
        public string Code { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public string BannerImage { get; set; } = string.Empty;
        public int PhoneCode { get; set; }
        public string Image { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool OnDoorPayment { get; set; }
        public string PrivacyPolicyText { get; set; } = string.Empty;
        public ICollection<CityVM> Cities { get; set; }

        public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, RequestResult<bool>>
        {
            private readonly ICountryService _countryService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            public CreateCountryCommandHandler(ICountryService countryService, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _countryService = countryService;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<RequestResult<bool>> Handle(CreateCountryCommand command, CancellationToken cancellationToken)
            {
                var country = new Country
                {
                    IsActive = command.IsActive,
                    BannerImage = command.BannerImage,
                    DeliveryLimitAmount = command.DeliveryLimitAmount,
                    DeliveryLimitPercent = command.DeliveryLimitPercent,
                    AverageDeliveryDays = command.AverageDeliveryDays,
                    DeliveryLimitFixedAmount = command.DeliveryLimitFixedAmount,
                    OnDoorPayment = command.OnDoorPayment,
                    PrivacyPolicyText = command.PrivacyPolicyText,
                };

                _countryService.CreateCountryData(country, command.Cities.Select(x => _mapper.Map<City>(x)).ToArray());
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
