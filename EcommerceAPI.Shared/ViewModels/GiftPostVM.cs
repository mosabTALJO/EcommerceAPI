using EcommerceAPI.Data.Models;

namespace EcommerceAPI.Shared.ViewModels
{
    public class GiftPostVM
    {
        public GiftType GiftType { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string? Image { get; set; }
        public int? CouponDiscount { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
    }
}
