using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Payments.Queries
{
    public class MyfatoorahSuccessQuery : IRequest
    {
        public string PaymentId { get; set; }
        public class MyfatoorahSuccessQueryHandler : IRequestHandler<MyfatoorahSuccessQuery>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly IOrderPaymentService _orderPaymentService;
            private readonly IUnitOfWork _unitOfWork;

            public MyfatoorahSuccessQueryHandler(IOrderService orderService, IUnitOfWork unitOfWork, IOrderPaymentService orderPaymentService, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _orderPaymentService = orderPaymentService;
                _orderLogService = orderLogService;
            }

            public async Task Handle(MyfatoorahSuccessQuery query, CancellationToken cancellationToken)
            {
                var p = await MyFatoorahHelper.GetPaymentStatusByPaymentId(query.PaymentId);
                Order o = null;
                if (p?.IsSuccess == true)
                {
                    var orderId = int.Parse(p.Data.CustomerReference);
                    o = _orderService.Queryable().FirstOrDefault(x => x.Id == orderId);
                    if (o != null)
                        o.PaymentId = query.PaymentId;
                }

                string path = "C:/inetpub/vhosts/api.trgatestore.com/httpdocs/Transactions/Success/" + query.PaymentId + " - " + Guid.NewGuid();
                if (p?.Data.InvoiceStatus == "Paid")
                {
                    if (o != null)
                    {
                        o.OrderStatus = OrderStatus.Paid;
                        o.PaidSalePrice = o.TotalSalePrice;
                        await _orderLogService.OrderPaiedLog(o.Id, o.CustomerId);

                        var orderPayment = await _orderPaymentService.Queryable().AsNoTracking().FirstOrDefaultAsync(x => x.PaymentId == query.PaymentId);
                        if (orderPayment != null)
                        {
                            _orderPaymentService.Insert(new OrderPayment
                            {
                                OrderId = o.Id,
                                Amount = o.PaidSalePrice ?? 0,
                                PaymentId = query.PaymentId,
                                InvoiceId = p.Data?.InvoiceId ?? 0,
                                InvoiceStatus = p.Data?.InvoiceStatus,
                                InvoiceURL = orderPayment?.InvoiceURL
                            });
                        }
                    }
                }
                else
                {
                    path = path.Replace("Success", "Failed");
                    if (o != null)
                        o.OrderStatus = OrderStatus.CartOrder;
                }
                await _unitOfWork.SaveChangesAsync();
                File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(p));
            }
        }
    }
}