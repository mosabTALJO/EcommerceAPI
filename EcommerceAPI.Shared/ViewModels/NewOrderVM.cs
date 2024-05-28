using EcommerceAPI.Data.Models;

namespace EcommerceAPI.Shared.ViewModels
{
    public class NewOrderVM
    {
        public int Id { get; set; }
        public int UserAddressId { get; set; }
        public string CouponCode { get; set; }
        public ICollection<NewOrderItemVM> OrderItems { get; set; }
        public int? UserCardId { get; set; }
        public OrderPaymentType PaymentType { get; set; }
    }


    public class PaymentVM
    {
        public int UserAddressId { get; set; }
    }
}
