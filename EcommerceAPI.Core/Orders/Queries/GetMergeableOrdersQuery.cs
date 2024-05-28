using MediatR;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Services;
using EcommerceAPI.Data.Models;

namespace EcommerceAPI.Core.Orders.Queries
{
    public class GetMergeableOrdersQuery : IRequest<RequestResult<List<int>>>
    {
        public int Id { get; set; }
        public class GetMergeableOrdersQueryHandler : IRequestHandler<GetMergeableOrdersQuery, RequestResult<List<int>>>
        {
            private readonly IOrderService _orderService;

            public GetMergeableOrdersQueryHandler(IOrderService orderService)
            {
                _orderService = orderService;
            }

            public async Task<RequestResult<List<int>>> Handle(GetMergeableOrdersQuery query, CancellationToken cancellationToken)
            {
                var item = await _orderService.GetOrderDetails(query.Id);
                if (item is null)
                    return new RequestResult<List<int>>(new List<int>());

                if (!(item.OrderStatus > OrderStatus.CartOrder))
                    return new RequestResult<List<int>>(new List<int>());

                var mergeableOrders = _orderService.Queryable().AsNoTracking().Include(o => o.Customer).Include(o => o.OrderItems)
                                                         .Where(o => !o.DeletedAt.HasValue)
                                                         .Where(o => o.CustomerId == item.CustomerId)
                                                         .Where(o => o.OrderStatus > OrderStatus.CartOrder)
                                                         .Where(o => o.OrderItems.All(x => x.OrderItemState == OrderItemState.Pending))
                                                         .Where(o => o.DiscountCoupunId == item.DiscountCoupunId)
                                                         .Where(o => o.Currency == item.Currency);
               
                if (mergeableOrders == null || mergeableOrders.Count() == 0)
                    return new RequestResult<List<int>>(new List<int>());

                var mergeableOrderIds = new List<int>();
                foreach (var order in mergeableOrders.ToArray())
                    mergeableOrderIds.Add(order.Id);
                mergeableOrderIds.Remove(query.Id);

                return new RequestResult<List<int>>(mergeableOrderIds);
            }
        }
    }
}
