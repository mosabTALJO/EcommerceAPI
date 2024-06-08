using EcommerceAPI.Data.Models;

namespace EcommerceAPI.Shared.ViewModels
{
    public class OrderLiteVM
    {
        public int Id { get; set; }
        public string ProductImages { get; set; }
        public string CustomerName { get; set; }
        public double TotalSalePrice { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public string City { get; set; }
        public OrderPaymentType PaymentType { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsArchived { get; set; }
    }
}