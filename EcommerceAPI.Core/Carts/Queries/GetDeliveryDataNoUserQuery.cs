using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Carts.Queries
{
    public class GetDeliveryDataNoUserQuery : IRequest<RequestResult<DeliveryData>>
    {
        public string CountryCode { get; set; }
        public string CityCode { get; set; }
        public CartItemVM[] Items { get; set; }
        public class GetDeliveryDataNoUserQueryHandler : IRequestHandler<GetDeliveryDataNoUserQuery, RequestResult<DeliveryData>>
        {
            private readonly ICountryService _countryService;
            private readonly IProductVariantService _productVariantService;

            public GetDeliveryDataNoUserQueryHandler(ICountryService countryService, IProductVariantService productVariantService)
            {
                _countryService = countryService;
                _productVariantService = productVariantService;
            }

            public async Task<RequestResult<DeliveryData>> Handle(GetDeliveryDataNoUserQuery query, CancellationToken cancellationToken)
            {
                var c = _countryService.Queryable().AsNoTracking().Include(x => x.Cities).FirstOrDefault(x => x.Code == query.CountryCode);
                var productVariantIds = query.Items.Select(x => x.ProductVariantId).ToList();
                var onlinePaymentOnly = await _productVariantService.Queryable().AnyAsync(x => productVariantIds.Contains(x.Id) && x.Product!.OnlinePaymentOnly);
                var city = c.Cities.FirstOrDefault(x => x.Code == query.CityCode);

                var deliverData = new DeliveryData
                {
                    FreeDeliveryAbove = c?.DeliveryLimitAmount.ToString() ?? "30",
                    DeliveryAverageDays = c?.AverageDeliveryDays.ToString() ?? "15",
                    CurrencyCode = c?.CurrencyCode ?? "KWD",
                    MyFatoorah = true,//c.MyFatoorahPayment,
                    OnDoorPayment = c.OnDoorPayment && !onlinePaymentOnly,
                };
                if (deliverData.OnDoorPayment)
                {
                    deliverData.RequestIdCard = true;
                    deliverData.IdCardTermsAndConditions = c.PrivacyPolicyText;
                }

                return new RequestResult<DeliveryData>(deliverData);
            }
        }
    }
}
