using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Data.Models
{
    public class Country
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string NameAr { get; set; }
        [MaxLength(200)]
        public string NameEn { get; set; }
        [MaxLength(200)]
        public string NameTr { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }
        [MaxLength(999)]
        public int PhoneCode { get; set; }
        [MaxLength(3)]
        public string CurrencyCode { get; set; } = string.Empty;
        [MaxLength(90)]
        public string BannerImage { get; set; }
        public bool IsActive { get; set; }
        public double DeliveryLimitAmount { get; set; }
        public double DeliveryLimitFixedAmount { get; set; }
        public double DeliveryLimitPercent { get; set; }
        public int AverageDeliveryDays { get; set; }
        public bool OnDoorPayment { get; set; }
        public string PrivacyPolicyText { get; set; }
        [MaxLength(60)]
        public string Image { get; set; }
        public bool IsDefault { get; set; }
        public ICollection<City> Cities { get; set; }
    }
}