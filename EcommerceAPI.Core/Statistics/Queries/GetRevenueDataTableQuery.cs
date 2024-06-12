using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Statistics.Queries
{
    public class GetRevenueDataTableQuery : IRequest<RequestResult<RevenueDataTable[]>>
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public class GetRevenueDataTableQueryHandler : IRequestHandler<GetRevenueDataTableQuery, RequestResult<RevenueDataTable[]>>
        {
            private readonly IOrderService _ordersService;
            private readonly ICountryService _countryService;

            public GetRevenueDataTableQueryHandler(IOrderService ordersService, ICountryService countryService)
            {
                _ordersService = ordersService;
                _countryService = countryService;
            }

            public async Task<RequestResult<RevenueDataTable[]>> Handle(GetRevenueDataTableQuery request, CancellationToken cancellationToken)
            {
                var gList = _ordersService.Queryable().Include(x => x.OrderItems)
                    .OrderBy(x => x.CreatedAt)
                    .Where(x => x.OrderStatus > OrderStatus.Pending)
                    .Where(x => request.From == null || x.CreatedAt >= request.From)
                    .Where(x => request.To == null || x.CreatedAt <= request.To)
                    .GroupBy(g => g.Currency)
                    .Select(x => new RevenueDataTable
                    {
                        Currency = x.Key,
                        CountryName = x.Key,
                        TotalDeliveryPrice = x.Sum(x => x.DeliveryPrice),
                        TotalOrders = x.Count(),
                        TotalCosts = x.Sum(x => x.TotalSalePrice - x.SubTotalSalePrice),
                        TotalSales = x.Sum(x => x.TotalSalePrice)
                    })
                    .ToArray();

                return new RequestResult<RevenueDataTable[]>(gList);
            }
        }
    }
    public class RevenueDataTable
    {
        public string CountryName { get; set; }
        public string Currency { get; set; }
        public int TotalOrders { get; set; }
        public double TotalDeliveryPrice { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalSales { get; set; }
        public double TotalCosts { get; set; }
        public double TotalRevenue => TotalSales - TotalCosts;
    }
}
