using EcommerceAPI.Data.Models;

namespace EcommerceAPI.Shared.ViewModels
{
    public class OrderItemVM
    {
        public int Id { get; set; }
        public ProductVariantVM ProductVariant { get; set; }
        public OrderItemState OrderItemState { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? RejectedDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public string ShippingCode { get; set; }
        public string ShippingCompany { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? ReturningDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public string Currency { get; set; }
        public string ReturnReason { get; set; }
        public double SalePrice { get; set; }
        public string ProductImage { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public double ProductSalesPrice { get; set; }
        public double? ProductDiscount { get; set; }
        public int Qty { get; set; }
        public double TotalSalePrice { get; set; }
        public double? CoupunDiscount { get; set; }
        public int OrderId { get; set; }
        public string ProductUrl { get; set; }
        public string OrderNumber { get; set; }
        public double SalePriceNoKDV { get; set; }
        public double KDV { get; set; }
        public double DiscountAmount { get; set; }
        public int? ShipmentId { get; set; }
        public bool HasLogs { get; set; }
        public double? CostPrice { get; set; }
    }
}