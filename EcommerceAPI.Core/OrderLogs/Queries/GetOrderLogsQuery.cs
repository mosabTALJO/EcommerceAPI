using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.OrderLogs.Queries
{
    public class GetOrderLogsQuery : IRequest<RequestResult<OrderLogVM[]>>
    {
        public int OrderId { get; set; }
        public class GetOrderLogsQueryHandler : IRequestHandler<GetOrderLogsQuery, RequestResult<OrderLogVM[]>>
        {
            private readonly IOrderLogService _orderLogService;
            private readonly IOrderService _orderService;
            private readonly IMapper _mapper;
            public GetOrderLogsQueryHandler(IOrderLogService orderLogService, IMapper mapper, IOrderService orderService)
            {
                _orderLogService = orderLogService;
                _mapper = mapper;
                _orderService = orderService;
            }
            public async Task<RequestResult<OrderLogVM[]>> Handle(GetOrderLogsQuery query, CancellationToken cancellationToken)
            {
                var itemIds = await _orderService.Queryable().AsNoTracking().Include(x => x.OrderItems).Where(x => x.Id == query.OrderId).SelectMany(x => x.OrderItems.Select(x => x.Id)).ToListAsync();
                return new RequestResult<OrderLogVM[]>(await _orderLogService.Queryable().AsNoTracking()
                    .Where(x => x.OrderId == query.OrderId || x.OrderItemId.HasValue && itemIds.Contains(x.OrderItemId.Value)).To<OrderLogVM>(_mapper).ToArrayAsync());
            }
        }
    }
}
