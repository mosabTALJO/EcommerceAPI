namespace EcommerceAPI.Shared.ViewModels
{
    public class GiftLiteVM
    {
        public int Id { get; set; }
        public string GiftType { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string? Image { get; set; }
        public double? CouponDiscount { get; set; }
    }
}
