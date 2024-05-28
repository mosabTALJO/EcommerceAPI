namespace EcommerceAPI.Shared.ViewModels
{
    public class CountryVM
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string NameTr { get; set; }
        public double DeliveryLimitAmount { get; set; }
        public double DeliveryLimitFixedAmount { get; set; }
        public double DeliveryLimitPercent { get; set; }
        public int AverageDeliveryDays { get; set; }
        public string Code { get; set; }
        public string CurrencyCode { get; set; }
        public string BannerImage { get; set; }
        public int PhoneCode { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public bool OnDoorPayment { get; set; }
        public bool MyFatoorahPayment { get; set; }
        public bool IsRequestIdCard { get; set; }
        public string PrivacyPolicyText { get; set; }
        public ICollection<CityVM> Cities { get; set; }
        public ICollection<GiftPriceRangeVM> GiftPriceRanges { get; set; }
    }
}