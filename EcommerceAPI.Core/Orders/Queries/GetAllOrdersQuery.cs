using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using EcommerceAPI.Shared.ViewModels;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Services;

namespace EcommerceAPI.Core.Orders.Queries
{
    public class GetAllOrdersQuery : IRequest<RequestResult<ListResponse<OrderLiteVM>>>
    {
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string customerId { get; set; } = null;
        public string Search { get; set; } = null;
        public string Country { get; set; } = null;
        public string City { get; set; } = null;
        public string SortField { get; set; } = "Id";
        public string SortOrder { get; set; } = "Asc";
        public string OrderStatus { get; set; } = null;
        public bool ShowArchived { get; set; } = false;
        public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, RequestResult<ListResponse<OrderLiteVM>>>
        {
            private readonly IOrderService _orderService;
            private readonly ICountryService _countryService;
            public GetAllOrdersQueryHandler(IOrderService orderService, ICountryService countryService)
            {
                _orderService = orderService;
                _countryService = countryService;
            }

            public async Task<RequestResult<ListResponse<OrderLiteVM>>> Handle(GetAllOrdersQuery query, CancellationToken cancellationToken)
            {
                var lst = _orderService.Queryable().AsNoTracking().Include(x => x.Customer)
                    .Where(x => !x.DeletedAt.HasValue)
                    .Where(o => o.IsArchived == query.ShowArchived);

                if (!string.IsNullOrEmpty(query.customerId))
                    lst = lst.Where(x => x.CustomerId == query.customerId);
                if (!string.IsNullOrEmpty(query.Search))
                    lst = lst.Include(x => x.OrderItems)
                        .Where(x => x.Id.ToString() == query.Search ||
                        x.PhoneNumber.Contains(query.Search) ||
                        x.RecipientFirstName.Contains(query.Search) ||
                        x.RecipientLastName.Contains(query.Search) ||
                        x.Customer.DisplayName.Contains(query.Search) ||
                        x.Customer.PhoneNumber.Contains(query.Search) ||
                        x.OrderItems.Any(x => x.OrderNumber == query.Search));
                if (!string.IsNullOrEmpty(query.Country))
                    lst = lst.Where(x => x.Country == query.Country);
                if (!string.IsNullOrEmpty(query.City))
                    lst = lst.Where(x => x.City == query.City);
                if (!string.IsNullOrEmpty(query.OrderStatus) && query.OrderStatus.ToLower() != "nan" && query.OrderStatus != "0")
                {
                    var os = (GetOrderStatus)int.Parse(query.OrderStatus);
                    if (os == GetOrderStatus.Completed || os == GetOrderStatus.Cart)
                        lst = lst.Include(x => x.OrderItems).Where(x => x.OrderStatus == Data.Models.OrderStatus.CartOrder && x.OrderItems.Any());
                    else
                        lst = lst.Where(x => x.OrderStatus > Data.Models.OrderStatus.Pending);

                    if (os == GetOrderStatus.Paid)
                        lst = lst.Where(x => x.OrderStatus == Data.Models.OrderStatus.Paid);
                    if (os == GetOrderStatus.ConfirmedForOnDoorPayment)
                        lst = lst.Where(x => x.OrderStatus == Data.Models.OrderStatus.ConfirmedForOnDoorPayment);
                    else if (os == GetOrderStatus.Pending)
                        lst = lst.Include(x => x.OrderItems).Where(x => x.OrderItems.Any(od => od.OrderItemState == Data.Models.OrderItemState.Pending));
                    else if (os == GetOrderStatus.Accepted)
                        lst = lst.Include(x => x.OrderItems).Where(x => x.OrderItems.Any(od => od.OrderItemState == Data.Models.OrderItemState.Accepted));
                    else if (os == GetOrderStatus.Rejected)
                        lst = lst.Include(x => x.OrderItems).Where(x => x.OrderItems.Any(od => od.OrderItemState == Data.Models.OrderItemState.Rejected));
                    else if (os == GetOrderStatus.Shipped)
                        lst = lst.Include(x => x.OrderItems).Where(x => x.OrderItems.Any(od => od.OrderItemState == Data.Models.OrderItemState.Shipped));
                    else if (os == GetOrderStatus.Delivered)
                        lst = lst.Include(x => x.OrderItems).Where(x => x.OrderItems.Any(od => od.OrderItemState == Data.Models.OrderItemState.Delivered));
                }
                else
                {
                    lst = lst.Where(x => x.OrderStatus > Data.Models.OrderStatus.Pending);
                }

                var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";

                var countries = await _countryService.GetCountries();

                var lstResult = lst.Select(x => new OrderLiteVM
                {
                    Currency = x.Currency,
                    City = x.City,
                    Country = x.Country,
                    Id = x.Id,
                    CustomerName = x.Customer.DisplayName,
                    TotalSalePrice = x.TotalSalePrice,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    PaymentType = x.PaymentType,
                    OrderStatus = x.OrderStatus,
                    IsArchived = x.IsArchived,
                });
                //lstResult = query.SortOrder?.ToLower() == "asc" ? lstResult.OrderBy(query.SortField) : lstResult.OrderByDescending(query.SortField);
                lstResult = lstResult.OrderByDescending(x => x.CreatedAt);

                var items = await lstResult.Skip(query.PageSize * query.PageNumber).Take(query.PageSize).ToArrayAsync();

                foreach (var i in items)
                {
                    i.Country = countries.FirstOrDefault(x => x.Code == i.Country)?.NameEn ?? i.Country;
                }

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
