using EcommerceAPI.Core.Carts;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Core.Orders
{
    public class OrderOperations
    {
        public static double CalculateOrderDeliveryPrice(double orderTotalWithoutDelivary, Country? country)
        {
            var freeDeliveryAmount = country?.DeliveryLimitAmount ?? 30;
            var deliveryLimitPercent = country?.DeliveryLimitPercent ?? 15;
            var deliveryLimitFixedAmount = country?.DeliveryLimitFixedAmount ?? 10;

           return orderTotalWithoutDelivary >= freeDeliveryAmount ? 0 : deliveryLimitFixedAmount + Math.Round(orderTotalWithoutDelivary * deliveryLimitPercent / 100, 3);
        }
        public static OrderItem CloneItem(OrderItem itemToClone)
        {
            return new OrderItem
            {
                Id = 0,
                AcceptedDate = itemToClone.AcceptedDate,
                CancelDate = itemToClone.CancelDate,
                CompleteRemove = itemToClone.CompleteRemove,
                CoupunDiscount = itemToClone.CoupunDiscount,
                CreatedAt = itemToClone.CreatedAt,
                CreatedBy = itemToClone.CreatedBy,
                DeletedAt = itemToClone.DeletedAt,
                DeletedBy = itemToClone.DeletedBy,
                DeliveredDate = itemToClone.DeliveredDate,
                DiscountAmount = itemToClone.DiscountAmount,
                EntityIdentifier = itemToClone.EntityIdentifier,
                KDV = itemToClone.KDV,
                ModifiedProperties = itemToClone.ModifiedProperties,
                Order = itemToClone.Order,
                OrderId = itemToClone.OrderId,
                OrderNumber = itemToClone.OrderNumber,
                OrderItemState = itemToClone.OrderItemState,
                ProductCompanyPrice = itemToClone.ProductCompanyPrice,
                ProductDiscount = itemToClone.ProductDiscount,
                ProductSalesPrice = itemToClone.ProductSalesPrice,
                ProductVariant = itemToClone.ProductVariant,
                ProductVariantId = itemToClone.ProductVariantId,
                ProductUrl = itemToClone.ProductUrl,
                Qty = itemToClone.Qty,
                RemainingQtyForDiscount = itemToClone.RemainingQtyForDiscount,
                ReturningDate = itemToClone.ReturningDate,
                ReturnReason = itemToClone.ReturnReason,
                SalePrice = itemToClone.SalePrice,
                SalePriceNoKDV = itemToClone.SalePriceNoKDV,
                //Shipment = itemToClone.Shipment,
                //ShipmentId = itemToClone.ShipmentId,
                ShippingCode = itemToClone.ShippingCode,
                ShippingCompany = itemToClone.ShippingCompany,
                ShippingDate = itemToClone.ShippingDate,
                TotalSalePrice = itemToClone.TotalSalePrice,
                TrackingState = itemToClone.TrackingState,
                UpdateCreatedAt = itemToClone.UpdateCreatedAt,
                UpdatedAt = itemToClone.UpdatedAt,
                UpdatedBy = itemToClone.UpdatedBy
            };
        }
        public static double CheckOrderTotalPriceLessThanZero(Order o)
        {
            if(o.TotalSalePrice < 0) return 0;
            return o.TotalSalePrice;
        }
    }
}
