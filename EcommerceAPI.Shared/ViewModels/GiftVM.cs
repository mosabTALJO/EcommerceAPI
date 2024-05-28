using EcommerceAPI.Data.Models;

namespace EcommerceAPI.Shared.ViewModels
{
    public class GiftVM
    {
        public int Id { get; set; }
        public GiftType GiftType { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string? Image { get; set; }
        public double? CouponDiscount { get; set; }
        public int? CouponId { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }
        public int GiftPriceRangeId { get; set; }
    }
}
