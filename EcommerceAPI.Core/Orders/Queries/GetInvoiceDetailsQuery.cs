using AutoMapper;
using MediatR;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;

namespace EcommerceAPI.Core.Orders.Queries
{
    public class GetInvoiceDetailsQuery : IRequest<RequestResult<PaymentDataModel>>
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public class GetInvoiceDetailsQueryHandler : IRequestHandler<GetInvoiceDetailsQuery, RequestResult<PaymentDataModel>>
        {
            private readonly IOrderService _orderService;
            private readonly IMapper _mapper;
            private readonly IRatingService _ratingService;

            public GetInvoiceDetailsQueryHandler(IOrderService orderService, IMapper mapper, IRatingService ratingService)
            {
                _orderService = orderService;
                _mapper = mapper;
                _ratingService = ratingService;
            }

            public async Task<RequestResult<PaymentDataModel>> Handle(GetInvoiceDetailsQuery query, CancellationToken cancellationToken)
            {
                var item = await _orderService.GetOrderDetails(query.OrderId);
                if (item is null)
                    return new RequestResult<PaymentDataModel>(ActionResultType.NotFound, "Order doesn't exist");
                if (string.IsNullOrEmpty(item.PaymentId))
                    return new RequestResult<PaymentDataModel>(ActionResultType.NotFound, "PaymentId doesn't exist");

                var payment = await MyFatoorahHelper.GetPaymentStatusByPaymentId(item.PaymentId);
                return new RequestResult<PaymentDataModel>(payment?.Data);
            }
        }
    }
}
