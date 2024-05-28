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
            private readonly IProductService _productService;

            public GetDeliveryDataNoUserQueryHandler(ICountryService countryService, IProductService productService)
            {
                _countryService = countryService;
                _productService = productService;
            }

            public async Task<RequestResult<DeliveryData>> Handle(GetDeliveryDataNoUserQuery query, CancellationToken cancellationToken)
            {
                var c = _countryService.Queryable().AsNoTracking().Include(x => x.Cities).FirstOrDefault(x => x.Code == query.CountryCode);
                var productIds = query.Items.Select(x => x.ProductId).ToList();
                var products = await _productService.Queryable().AsNoTracking().Where(x => productIds.Contains(x.Id)).ToArrayAsync();
                var city = c.Cities.FirstOrDefault(x => x.Code == query.CityCode);

                var deliverData = new DeliveryData
                {
                    FreeDeliveryAbove = c?.DeliveryLimitAmount.ToString() ?? "30",
                    DeliveryAverageDays = c?.AverageDeliveryDays.ToString() ?? "15",
                    CurrencyCode = c?.CurrencyCode ?? "KWD",
                    MyFatoorah = c.MyFatoorahPayment,
                    OnDoorPayment = c.OnDoorPayment && !products.Any(x => x.OnlinePaymentOnly) && city?.IsBlackList != true,
                };
                if (deliverData.OnDoorPayment && c.IsRequestIdCard == true)
                {
                    deliverData.RequestIdCard = true;
                    deliverData.IdCardTermsAndConditions = c.PrivacyPolicyText;
                }

                return new RequestResult<DeliveryData>(deliverData);
            }
        }
    }
}
