using System.ComponentModel.DataAnnotations.Schema;
using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class OrderItem : SoftDeleteEntity
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        public OrderItemState OrderItemState { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public string ShippingCompany { get; set; } = string.Empty;
        public string ShippingCode { get; set; } = string.Empty;
        public DateTime? DeliveredDate { get; set; }
        public DateTime? ReturningDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public DateTime? RejectedDate { get; set; }
        public DateTime? InStockDate { get; set; }
        public string ReturnReason { get; set; } = string.Empty;
        public double SalePrice { get; set; }
        public double ProductCompanyPrice { get; set; }
        public double ProductSalesPrice { get; set; }
        public double? ProductDiscount { get; set; }
        public int Qty { get; set; }
        public double TotalSalePrice { get; set; }
        public double? CoupunDiscount { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string ProductUrl { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public double SalePriceNoKDV { get; set; }
        public double KDV { get; set; }
        public double DiscountAmount { get; set; }
        public virtual ICollection<OrderLog> Logs { get; set; }
        [NotMapped]
        public bool HasLogs { get => Logs?.Any() ?? false; }
        [NotMapped]
        public int RemainingQtyForDiscount { get; set; }
    }
}
