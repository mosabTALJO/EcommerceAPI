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
            private readonly IGiftPriceRangeService _giftPriceRangeService;
            public UpdateMultipleCountriesCommandHandler(ICountryService countryService, IUnitOfWork unitOfWork, IMapper mapper, IGiftPriceRangeService giftPriceRangeService)
            {
                _countryService = countryService;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _giftPriceRangeService = giftPriceRangeService;
            }
            public async Task<RequestResult<bool>> Handle(UpdateMultipleCountriesCommand command, CancellationToken cancellationToken)
            {
                foreach (var country in command.Countries)
                {
                    var dbCountry = await _countryService.Queryable().AsNoTracking().Include(c => c.GiftPriceRanges).ThenInclude(gr => gr.Gifts).FirstAsync(x => x.Id == country.Id);
                    dbCountry.IsActive = country.IsActive;
                    dbCountry.BannerImage = country.BannerImage;
                    dbCountry.DeliveryLimitAmount = country.DeliveryLimitAmount;
                    dbCountry.DeliveryLimitPercent = country.DeliveryLimitPercent;
                    dbCountry.AverageDeliveryDays = country.AverageDeliveryDays;
                    dbCountry.DeliveryLimitFixedAmount = country.DeliveryLimitFixedAmount;
                    dbCountry.OnDoorPayment = country.OnDoorPayment;
                    dbCountry.MyFatoorahPayment = country.MyFatoorahPayment;
                    dbCountry.IsRequestIdCard = country.IsRequestIdCard;
                    dbCountry.PrivacyPolicyText = country.PrivacyPolicyText;

                    var newGiftRanges = new List<GiftPriceRange>();
                    country.GiftPriceRanges ??= [];
                    foreach (var giftRangeVM in country.GiftPriceRanges)
                    {
                        var giftRange = _mapper.Map<GiftPriceRange>(giftRangeVM);
                        if (dbCountry.GiftPriceRanges.Any(gr => gr.Id == giftRange.Id))
                        {
                            await _giftPriceRangeService.UpdateGiftPriceRange(giftRange, giftRange.Gifts.ToList());
                            newGiftRanges.Add(giftRange);
                        }
                        else
                        {
                            giftRange.CountryId = dbCountry.Id;
                            await _giftPriceRangeService.CreateGiftPriceRange(giftRange);
                            newGiftRanges.Add(giftRange);
                        }
                    }

                    dbCountry.GiftPriceRanges ??= new List<GiftPriceRange>();
                    foreach (var giftRange in dbCountry.GiftPriceRanges)
                    {
                        if (!country.GiftPriceRanges.Any(gr => gr.Id == giftRange.Id))
                        {
                            await _giftPriceRangeService.DeleteGiftPriceRange(giftRange);
                        }
                    }
                    dbCountry.GiftPriceRanges = newGiftRanges;

                    _countryService.UpdateCountryData(dbCountry, country.Cities.Select(x => _mapper.Map<City>(x)).ToArray());
                }

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<bool>(true);
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}