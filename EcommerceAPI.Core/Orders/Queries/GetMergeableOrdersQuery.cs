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
                var order = await _orderService.GetOrderDetails(query.Id);
                if (order is null)
                    return new RequestResult<List<int>>(new List<int>());

                if ((order.OrderStatus != OrderStatus.Paid && order.OrderStatus != OrderStatus.ConfirmedForOnDoorPayment) || !order.OrderItems.All(x => x.OrderItemState == OrderItemState.Pending))
                    return new RequestResult<List<int>>(new List<int>());

                var mergeableOrderIds = await _orderService.Queryable().AsNoTracking().Include(o => o.Customer).Include(o => o.OrderItems)
                                                         .Where(o => !o.DeletedAt.HasValue)
                                                         .Where(o => o.CustomerId == order.CustomerId)
                                                         .Where(o => o.OrderStatus > OrderStatus.CartOrder)
                                                         .Where(o => o.OrderItems.All(x => x.OrderItemState == OrderItemState.Pending))
                                                         .Where(o => o.DiscountCoupunId == order.DiscountCoupunId)
                                                         .Where(o => o.Currency == order.Currency)
                                                         .Select(x => x.Id)
                                                         .ToListAsync();
               
                if (mergeableOrderIds == null || mergeableOrderIds.Count == 0)
                    return new RequestResult<List<int>>(new List<int>());

                mergeableOrderIds.Remove(query.Id);

                return new RequestResult<List<int>>(mergeableOrderIds);
            }
        }
    }
}