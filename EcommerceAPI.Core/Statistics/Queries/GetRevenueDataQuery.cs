using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Statistics.Queries
{
    public class GetRevenueDataQuery : IRequest<RequestResult<RevenueChart>>
    {
        public int CountryId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public class GetRevenueDataQueryHandler : IRequestHandler<GetRevenueDataQuery, RequestResult<RevenueChart>>
        {
            private readonly IOrderService _ordersService;
            private readonly ICountryService _countryService;

            public GetRevenueDataQueryHandler(IOrderService ordersService, ICountryService countryService)
            {
                _ordersService = ordersService;
                _countryService = countryService;
            }

            public async Task<RequestResult<RevenueChart>> Handle(GetRevenueDataQuery request, CancellationToken cancellationToken)
            {
                Country c = null;
                if (request.CountryId > 0)
                {
                    c = _countryService.Queryable().FirstOrDefault(x => x.Id == request.CountryId);
                    c.CurrencyCode = c.CurrencyCode ?? "kwd";
                }
                var gList = _ordersService.Queryable().Include(x => x.OrderItems)
                    .OrderBy(x => x.CreatedAt)
                    .Where(x => x.OrderStatus > OrderStatus.Pending)
                    .Where(x => c == null || c.CurrencyCode.ToLower() == x.Currency.ToLower())
                    .Where(x => request.From == null || x.CreatedAt >= request.From)
                    .Where(x => request.To == null || x.CreatedAt <= request.To)
                    .Select(x => new { x.CurrencyRate, x.CreatedAt, x.DeliveryPrice, x.TotalSalePrice, x.SubTotalSalePrice, x.OrderItems })
                    .ToArray()
                    .GroupBy(g => new { g.CreatedAt.Year, g.CreatedAt.Month }).ToArray();

                var chart = new RevenueChart();
                foreach (var item in gList)
                {
                    chart.Titles.Add(item.Key.Year + " - " + item.Key.Month);
                    chart.Sales.Add(Math.Round(item.Sum(x => x.OrderItems.Sum(x => x.ProductSalesPrice)), 2));
                    chart.Revenue.Add(Math.Round(item.Sum(x => x.TotalSalePrice), 2));
                }
                return new RequestResult<RevenueChart>(chart);
            }
        }
    }
}
