using EcommerceAPI.Data.Models;

namespace EcommerceAPI.Shared.ViewModels
{
    public class OrderVM
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public string PaymentId { get; set; }
        public string InvoiceId { get; set; }
        public string CustomerName { get; set; }
        public double TotalSalePrice { get; set; }
        public double TotalCancelledPrice { get; set; }
        public double DeliveryPrice { get; set; }
        public double? PaidSalePrice { get; set; }
        public int? DiscountCoupunId { get; set; }
        public double? CoupunDiscountRate { get; set; }
        public string CoupunName { get; set; }
        public string Currency { get; set; }
        public double CurrencyRate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public OrderPaymentType PaymentType { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string FullAddress { get; set; }
        public string RecipientFirstName { get; set; }
        public string RecipientLastName { get; set; }
        public string PhoneNumber { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PaymentGateway? PaymentGateway { get; set; }
        public ICollection<OrderItemVM> OrderItems { get; set; }
        public double SubTotalSalePrice { get; set; }
        public string GeneratedPaymentLinks { get; set; }
        public bool IsArchived { get; set; }
        public double DiscountAmount { get; set; }
        public RatingVM[] Ratings { get; set; }
        public bool CanRate { get; set; } = true;
        public OrderDeviceType? OrderDeviceType { get; set; }
        public ICollection<OrderPaymentVM> OrderPayments { get; set; }
        public double SpecialDiscount { get; set; }
        public CaptureStatus CaptureStatus { get; set; }
    }
}
