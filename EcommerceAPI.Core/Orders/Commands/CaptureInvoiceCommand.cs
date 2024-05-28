using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class CaptureInvoiceCommand : IRequest<RequestResult<bool>>
    {
        public int OrderId { get; set; }
        public class CaptureInvoiceCommandHandler : IRequestHandler<CaptureInvoiceCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly ICountryService _countryService;
            private readonly IUnitOfWork _unitOfWork;
            public CaptureInvoiceCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, ICountryService countryService, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _countryService = countryService;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(CaptureInvoiceCommand command, CancellationToken cancellationToken)
            {
                var o = _orderService.Queryable().FirstOrDefault(x => x.Id == command.OrderId);

                if (o is null)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Order doesn't exist");
                if (string.IsNullOrEmpty(o.PaymentId))
                    return new RequestResult<bool>(ActionResultType.NotFound, "PaymentId doesn't exist");
                if (o.CaptureStatus != CaptureStatus.Pending)
                    return new RequestResult<bool>(ActionResultType.NotFound, "Invoice already captured!");

                var payment = await MyFatoorahHelper.GetPaymentStatusByPaymentId(o.PaymentId);
                var capture = await MyFatoorahHelper.UpdatePaymentStatus(o.PaymentId, payment.Data.InvoiceValue);
                if (capture.IsSuccess)
                {
                    o.CaptureStatus = CaptureStatus.Captured;
                    await _unitOfWork.SaveChangesAsync();
                    return new RequestResult<bool>(true);
                }
                return new RequestResult<bool>(ActionResultType.BadRequest, "An error occured while capturing invoice: " + capture.Message);
            }
        }
    }
}
