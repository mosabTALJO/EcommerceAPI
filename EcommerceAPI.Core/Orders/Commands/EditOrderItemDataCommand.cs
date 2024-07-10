using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class EditOrderItemDataCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
        public string? UserId { get; set; }
        public class EditOrderItemDataCommandHandler : IRequestHandler<EditOrderItemDataCommand, RequestResult<bool>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICountryService _countryService;
            private readonly ICollectionService _collectionService;
            private readonly UserManager<AppUser> _userManager;
            public EditOrderItemDataCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, ICountryService countryService, UserManager<AppUser> userManager, ICollectionService collectionService, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _countryService = countryService;
                _userManager = userManager;
                _collectionService = collectionService;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<bool>> Handle(EditOrderItemDataCommand command, CancellationToken cancellationToken)
            {
                try
                {
                    var order = await _orderService.GetOrderDetails(command.OrderId);
                    var orderItem = order?.OrderItems.FirstOrDefault(oi => oi.Id == command.Id);
                    if (orderItem == null)
                        return new RequestResult<bool>(ActionResultType.NotFound, "order item not found in order");

                    if (orderItem.OrderItemState <= OrderItemState.Accepted && orderItem.Order.OrderStatus != OrderStatus.Shipped && orderItem.Order.OrderStatus != OrderStatus.Completed)
                    {
                        var oldOrderItemPrice = orderItem.TotalSalePrice;
                        var oldPaidTotalPrice = order.TotalSalePrice;
                        var oldQty = orderItem.Qty;

                        order.TotalSalePrice -= orderItem.TotalSalePrice;
                        order.DiscountAmount -= orderItem.DiscountAmount;

                        orderItem.SalePrice = command.Price;
                        orderItem.Qty = command.Qty;
                        orderItem.SalePrice -= orderItem.SalePrice * (orderItem.CoupunDiscount ?? 0) / 100;

                        if (orderItem.DiscountAmount > 0 && orderItem.Qty < oldQty /*&& orderItem.RemainingQtyForDiscount == oldQty*/)
                            orderItem.DiscountAmount += Math.Round(Math.Ceiling(orderItem.Qty - oldQty / 2.0) * orderItem.SalePrice, 3);

                        orderItem.TotalSalePrice = Math.Round(orderItem.Qty * orderItem.SalePrice - orderItem.DiscountAmount, 3);

                        order.TotalSalePrice += orderItem.TotalSalePrice;
                        order.DiscountAmount += orderItem.DiscountAmount;

                        var orderTotalWithoutDelivary = order.TotalSalePrice - order.DeliveryPrice;

                        if (order.Currency == null)
                            return new RequestResult<bool>(ActionResultType.BadRequest, "order currency code is null");

                        var c = _countryService.Queryable().AsNoTracking().FirstOrDefault(x => x.CurrencyCode.ToLower() == order.Currency.ToLower()) ??
                        _countryService.Queryable().AsNoTracking().FirstOrDefault(x => x.CurrencyCode.ToLower() == "kwd");

                        order.DeliveryPrice = OrderOperations.CalculateOrderDeliveryPrice(orderTotalWithoutDelivary, c);

                        order.TotalSalePrice = orderTotalWithoutDelivary + order.DeliveryPrice;
                        order.TotalSalePrice = OrderOperations.CheckOrderTotalPriceLessThanZero(order);

                        if (order.PaymentType == OrderPaymentType.Online)
                        {
                            if (oldPaidTotalPrice < order.TotalSalePrice)
                            {

                            }
                            else
                            {
                                if (await _unitOfWork.SaveChangesAsync() < 0)
                                    return new RequestResult<bool>(ActionResultType.BadRequest, "an error occured while saving customer balance");
                            }
                        }

                        bool addLog = false;
                        string logNotes = $"Edit Order Item Data";
                        if (oldOrderItemPrice != orderItem.TotalSalePrice)
                        {
                            logNotes += "\nOld Item Price=" + oldOrderItemPrice;
                            addLog = true;
                        }
                        if (oldQty != orderItem.Qty)
                        {
                            logNotes += "\nOld Quantity=" + oldQty;
                            addLog = true;
                        }

                        if (addLog)
                            await _orderLogService.ItemUpdatedOrEditedLog(orderItem, command.UserId, logNotes);
                        _orderService.UpdateOrder(order);
                        if (await _unitOfWork.SaveChangesAsync() > 0)
                            return new RequestResult<bool>(true);
                        return new RequestResult<bool>(ActionResultType.BadRequest, "ERROR");
                    }
                    return new RequestResult<bool>(ActionResultType.BadRequest, "can't change this order item");
                }
                catch (Exception ex)
                {
                    return new RequestResult<bool>(ActionResultType.BadRequest, ex.Message + "--------\n" + ex.InnerException?.Message);
                }
            }
        }
    }
}
