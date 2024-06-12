using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using MediatR;

namespace EcommerceAPI.Core.Statistics.Queries;

public class GetOrdersDataQuery : IRequest<RequestResult<Chart>>
{
    public class OrdersDataQueryHandler : IRequestHandler<GetOrdersDataQuery, RequestResult<Chart>>
    {
        private readonly IOrderService _ordersService;
        public OrdersDataQueryHandler(IOrderService ordersService)
        {
            _ordersService = ordersService;
        }
        public async Task<RequestResult<Chart>> Handle(GetOrdersDataQuery request, CancellationToken cancellationToken)
        {
            var gList = _ordersService.Queryable().Where(x => x.OrderStatus > OrderStatus.Pending).ToArray().GroupBy(g => new { g.CreatedAt.Year, g.CreatedAt.Month }).ToArray();
            var chart = new Chart();
            foreach (var item in gList)
            {
                chart.Titles.Add(item.Key.Year + " - " + item.Key.Month);
                chart.Data.Add(item.Count());
            }
            return new RequestResult<Chart>(chart);
        }
    }
}
