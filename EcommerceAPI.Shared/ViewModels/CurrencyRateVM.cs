using EcommerceAPI.Data.Models;

namespace EcommerceAPI.Shared.ViewModels
{
    public class CurrencyRateLiteVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string CountryCode { get; set; }
    }
    public class CurrencyRateVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string CountryCode { get; set; }
        public double Rate { get; set; }
        public double FeeRate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }
}
