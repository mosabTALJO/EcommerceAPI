using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Services;
using EcommerceAPI.Data.Models;

namespace EcommerceAPI.Core.Orders.Queries
{
    public class GetMyOrdersQuery : IRequest<RequestResult<ListResponse<OrderLiteVM>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string UserId { get; set; }
        public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, RequestResult<ListResponse<OrderLiteVM>>>
        {
            private readonly IOrderService _orderService;
            private readonly ICountryService _countryService;
            public GetMyOrdersQueryHandler(IOrderService orderService, ICountryService countryService)
            {
                _orderService = orderService;
                _countryService = countryService;
            }
            public async Task<RequestResult<ListResponse<OrderLiteVM>>> Handle(GetMyOrdersQuery query, CancellationToken cancellationToken)
            {
                var lst = _orderService.Queryable()
                    .Include(x => x.OrderItems).ThenInclude(x => x.ProductVariant)
                    .Include(x => x.Customer)
                    .Where(x => !x.DeletedAt.HasValue && x.OrderStatus > OrderStatus.Pending && x.CustomerId == query.UserId);

                var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";
                var countries = await _countryService.GetCountries();

                var lstResult = lst.OrderByDescending(x => x.CreatedAt).Select(x => new OrderLiteVM
                {
                    Currency = x.Currency,
                    City = x.City,
                    Country = x.Country,
                    Id = x.Id,
                    CustomerName = x.Customer.DisplayName,
                    TotalSalePrice = x.TotalSalePrice,
                    CreatedAt = x.CreatedAt,
                    PaymentType = x.PaymentType,
                    OrderStatus = x.OrderStatus,
                });

                var items = await lstResult.Skip(query.PageSize * query.PageNumber).Take(query.PageSize).ToArrayAsync();
                foreach (var item in items)
                {
                    item.Country = countries.FirstOrDefault(x => x.Code == item.Country)?.NameEn ?? item.Country;
                };

                var result = new ListResponse<OrderLiteVM>
                {
                    TotalRecords = await lst.CountAsync(),
                    Items = items
                };
                return new RequestResult<ListResponse<OrderLiteVM>>(result);
            }
        }
    }
}
