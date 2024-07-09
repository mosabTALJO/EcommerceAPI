using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Carts.Queries
{
    public class GetDeliveryDataQuery : IRequest<RequestResult<DeliveryData>>
    {
        public int CountryId { get; set; }
        public string UserId { get; set; }
        public class GetDeliveryDataQueryHandler : IRequestHandler<GetDeliveryDataQuery, RequestResult<DeliveryData>>
        {
            private readonly ICountryService _countryService;
            private readonly IOrderService _orderService;
            private readonly UserManager<AppUser> _userManager;

            public GetDeliveryDataQueryHandler(ICountryService countryService, UserManager<AppUser> userManager, IOrderService orderService)
            {
                _countryService = countryService;
                _userManager = userManager;
                _orderService = orderService;
            }

            public async Task<RequestResult<DeliveryData>> Handle(GetDeliveryDataQuery query, CancellationToken cancellationToken)
            {
                var c = _countryService.Queryable().AsNoTracking().Include(x => x.Cities).FirstOrDefault(x => x.Id == query.CountryId);
                var u = await _userManager.FindByIdAsync(query.UserId);
                var onlinePaymentOnlyExists = await _orderService.ItemsQueryable()
                    .AnyAsync(x => x.Order.OrderStatus == OrderStatus.CartOrder && x.Order.CustomerId == query.UserId && x.ProductVariant!.Product!.OnlinePaymentOnly);

                bool OnDoorPayment = c?.OnDoorPayment == true && !onlinePaymentOnlyExists;
                var deliverData = new DeliveryData
                {
                    FreeDeliveryAbove = c?.DeliveryLimitAmount.ToString() ?? "30",
                    DeliveryAverageDays = c?.AverageDeliveryDays.ToString() ?? "15",
                    CurrencyCode = c?.CurrencyCode,
                    MyFatoorah = true,//c.MyFatoorahPayment,
                    OnDoorPayment = OnDoorPayment,
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
    public class DeliveryData
    {
        public string FreeDeliveryAbove { get; set; }
        public string DeliveryAverageDays { get; set; }
        public string CurrencyCode { get; set; }
        public bool MyFatoorah { get; set; }
        public bool OnDoorPayment { get; set; }
        public bool RequestIdCard { get; set; }
        public string IdCardTermsAndConditions { get; set; }
    }
}
