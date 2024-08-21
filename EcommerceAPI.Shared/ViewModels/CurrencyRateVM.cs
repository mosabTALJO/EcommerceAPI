using EcommerceAPI.Data.Models;

namespace EcommerceAPI.Shared.ViewModels
{
    public class CurrencyRateLiteVM
    {
        public int Id { get; set; }
        public required string FromCurrencyCode { get; set; }
        public required string ToCurrencyCode { get; set; }
        public double Rate { get; set; }
        public double FeeRate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }
    public class CurrencyRateVM
    {
        public int Id { get; set; }
        public required string FromCurrencyCode { get; set; }
        public required string ToCurrencyCode { get; set; }
        public double Rate { get; set; }
        public double FeeRate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }
}
