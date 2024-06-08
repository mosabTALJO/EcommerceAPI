using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class Order : SoftDeleteEntity
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public string PaymentId { get; set; }
        public string InvoiceId { get; set; }
        public AppUser Customer { get; set; }
        public double TotalSalePrice { get; set; }
        public double SubTotalSalePrice { get; set; }
        public double? PaidSalePrice { get; set; }
        public double DeliveryPrice { get; set; }
        public double? CoupunDiscountRate { get; set; }
        public int? DiscountCoupunId { get; set; }
        public string Currency { get; set; }
        public double CurrencyRate { get; set; }
        public PaymentGateway? PaymentGateway { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public OrderPaymentType PaymentType { get; set; }

        [MaxLength(200)]
        public string RecipientFirstName { get; set; }
        [MaxLength(200)]
        public string RecipientLastName { get; set; }
        [MaxLength(200)]
        public string Country { get; set; }
        [MaxLength(200)]
        public string City { get; set; }
        [MaxLength(2000)]
        public string FullAddress { get; set; }
        [MaxLength(30)]
        public string PhoneNumber { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public bool IsArchived { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public double DiscountAmount { get; set; }
        public OrderDeviceType? OrderDeviceType { get; set; }
        public virtual ICollection<OrderPayment> OrderPayments { get; set; }
        public double SpecialDiscount { get; set; }
        public CaptureStatus CaptureStatus { get; set; }
    }
    public enum CaptureStatus { Pending, Captured, Released }
    public enum PaymentGateway { Iyzico, MyFatoorah, PayTab }
    public enum OrderDeviceType
    {
        None = 0,
        Android = 1,
        IOS = 2,
        Web = 3,
        DashBoard = 4,
    }
    public enum OrderItemState
    {
        Pending,
        Accepted,
        Rejected,
        Shipped,
        Delivered,
        Returning,
        Returned,
        Cancelled,
        InStock,
    }
    public enum OrderStatus
    {
        CartOrder = -1,
        Pending,
        Paid,
        Shipped,
        Completed,
        ConfirmedForOnDoorPayment,
        PartiallyPaid,
        OnDoorPaymentPending
    }
    public enum OrderPaymentType
    {
        Online,
        OnDoor
    }
}