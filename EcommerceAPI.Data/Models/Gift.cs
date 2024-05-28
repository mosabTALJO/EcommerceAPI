using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class Gift : TrackableEntity
    {
        public int Id { get; set; }
        public GiftType GiftType { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string? Image { get; set; }
        public double? CouponDiscount { get; set; }
        public int? CouponId { get; set; }
        public Coupon Coupon { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public int GiftPriceRangeId { get; set; }
        public GiftPriceRange GiftPriceRange { get; set; }
    }

    public enum GiftType
    {
        Physical,
        Coupon
    }
}
