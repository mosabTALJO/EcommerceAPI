using AutoMapper;
using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.OrderLogs.Queries
{
    public class GetOrderItemLogsQuery : IRequest<RequestResult<OrderLogVM[]>>
    {
        public int OrderItemId { get; set; }
        public class GetOrderItemLogsQueryHandler : IRequestHandler<GetOrderItemLogsQuery, RequestResult<OrderLogVM[]>>
        {
            private readonly IOrderLogService _orderLogService;
            private readonly IMapper _mapper;
            public GetOrderItemLogsQueryHandler(IOrderLogService orderLogService, IMapper mapper)
            {
                _orderLogService = orderLogService;
                _mapper = mapper;
            }
            public async Task<RequestResult<OrderLogVM[]>> Handle(GetOrderItemLogsQuery query, CancellationToken cancellationToken)
            {
                return new RequestResult<OrderLogVM[]>(await _orderLogService.Queryable().AsNoTracking().Where(x => x.OrderItemId == query.OrderItemId).To<OrderLogVM>(_mapper).ToArrayAsync());
            }
        }
    }
}
