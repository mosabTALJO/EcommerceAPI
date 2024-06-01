using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders.Commands
{
    public class MergeOrdersCommand : IRequest<RequestResult<int>>
    {
        public int FirstId { get; set; }
        public int SecondId { get; set; }
        public string UserId { get; set; }
        public class MergeOrdersCommandHandler : IRequestHandler<MergeOrdersCommand, RequestResult<int>>
        {
            private readonly IOrderService _orderService;
            private readonly IOrderLogService _orderLogService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICountryService _countryService;

            public MergeOrdersCommandHandler(IOrderService orderService, IUnitOfWork unitOfWork, ICountryService countryService, IOrderLogService orderLogService)
            {
                _orderService = orderService;
                _unitOfWork = unitOfWork;
                _countryService = countryService;
                _orderLogService = orderLogService;
            }
            public async Task<RequestResult<int>> Handle(MergeOrdersCommand command, CancellationToken cancellationToken)
            {
                var order1 = await _orderService.GetOrderDetails(command.FirstId);
                var order2 = await _orderService.GetOrderDetails(command.SecondId);
                if (order1 is null || order2 is null)
                    return new RequestResult<int>(ActionResultType.NotFound, "NotFound");

                if (!(order1.OrderStatus == OrderStatus.Paid || order1.OrderStatus == OrderStatus.ConfirmedForOnDoorPayment) || !order1.OrderItems.All(x => x.OrderItemState == OrderItemState.Pending))
                    return new RequestResult<int>(ActionResultType.BadRequest, $"The order with id ({order1.Id}) is not mergeable");

                if (!(order2.OrderStatus == OrderStatus.Paid || order2.OrderStatus == OrderStatus.ConfirmedForOnDoorPayment) || !order2.OrderItems.All(x => x.OrderItemState == OrderItemState.Pending))
                    return new RequestResult<int>(ActionResultType.BadRequest, $"The order with id ({order2.Id}) is not mergeable");

                if (order1.DiscountCoupunId != order2.DiscountCoupunId)
                    return new RequestResult<int>(ActionResultType.BadRequest, "These orders have different coupons");

                if (order1.Currency != order2.Currency)
                    return new RequestResult<int>(ActionResultType.BadRequest, "These orders are paid in different currencies");

                string valueUpdatesItems = "";
                string MergedItems = "";
                foreach (var item1 in order1.OrderItems)
                {
                    var item2 = order2.OrderItems.FirstOrDefault(oi => oi.ProductVariantId == item1.ProductVariantId);
                    if (item2 != null)
                    {
                        item2.Qty += item1.Qty;
                        item2.TotalSalePrice += item1.TotalSalePrice;
                        await _orderLogService.ItemUpdatedOrEditedLog(item1, command.UserId, $"item {item1.Id} values in order {order1.Id} were added to item {item2.Id} values in order {order1.Id}.");
                        valueUpdatesItems += $"{item1.Id}, ";
                    }
                    else
                    {
                        //item1.Id = 0;
                        order2.OrderItems.Add(OrderOperations.CloneItem(item1));
                        await _orderLogService.ItemUpdatedOrEditedLog(item1, command.UserId, $"item {item1.Id} was added to order {order2.Id}.");
                        MergedItems += $"{item1.Id}, ";
                    }
                }
                string orderNote = $"order {order1.Id} was add to order {order2.Id}." +
                                   $"\nOld SubTotalSalePrice = {order2.SubTotalSalePrice}." +
                                   $"\nOld DeliveryPrice = {order2.DeliveryPrice}." +
                                   $"\nOld TotalSalePrice = {order2.TotalSalePrice}." +
                                   $"\nChanged items: {valueUpdatesItems}." +
                                   $"\nAdded items: {MergedItems}.";
                order2.SubTotalSalePrice += order1.SubTotalSalePrice;

                var item2TotalWithoutDelivary = order2.TotalSalePrice + order1.TotalSalePrice - order2.DeliveryPrice - order1.DeliveryPrice;

                var c = _countryService.Queryable().AsNoTracking().FirstOrDefault(x => x.CurrencyCode.ToLower() == order2.Currency.ToLower()) ??
                _countryService.Queryable().AsNoTracking().FirstOrDefault(x => x.CurrencyCode.ToLower() == "kwd");
                var freeDeliveryAmount = c?.DeliveryLimitAmount ?? 30;
                var deliveryLimitPercent = c?.DeliveryLimitPercent ?? 15;
                var deliveryLimitFixedAmount = c?.DeliveryLimitFixedAmount ?? 10;

                order2.DeliveryPrice = item2TotalWithoutDelivary >= freeDeliveryAmount ? 0 : deliveryLimitFixedAmount + Math.Round(item2TotalWithoutDelivary * deliveryLimitPercent / 100, 3);

                order2.TotalSalePrice = item2TotalWithoutDelivary + order2.DeliveryPrice;

                order2.TotalSalePrice =  OrderOperations.CheckOrderTotalPriceLessThanZero(order2);

                _orderService.UpdateOrder(order2);
                await _orderLogService.OrderUpdatedLog(order2, command.UserId, orderNote);
                var updateSuccess = await _unitOfWork.SaveChangesAsync();

                _orderService.DeleteOrder(order1);
                if (await _unitOfWork.SaveChangesAsync() > 0 && updateSuccess > 0)
                    return new RequestResult<int>(order2.Id);
                return new RequestResult<int>(ActionResultType.BadRequest, "An error occured while merging");
            }
        }
    }
}
