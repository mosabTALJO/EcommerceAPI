namespace EcommerceAPI.Data.Models
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public required string FromCurrencyCode { get; set; }
        public required string ToCurrencyCode { get; set; }
        public double Rate { get; set; }
        public double FeeRate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }
}
