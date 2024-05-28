using System.Text.Json.Serialization;

namespace EcommerceAPI.Shared.ViewModels
{
    public class ProductLiteVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BrandName { get; set; }
        public double SalesPrice { get; set; }
        public double DiscountedSalesPrice => Discount == null ? Math.Round(SalesPrice, 1) : Math.Round(SalesPrice * (1 - Discount.Value / 100), 1);
        public double FinalSalesPrice => Discount == null ? Math.Round(SalesPrice, 1) : Math.Round(SalesPrice * (1 - Discount.Value / 100), 1);
        public double? Discount { get; set; }
        [JsonIgnore]
        public string MainImageRow { get; set; }
        //public string MainImage => string.IsNullOrWhiteSpace(MainImageRow) || MainImageRow.StartsWith("https") ? MainImageRow : "";
        public double? TotalRate { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsActive { get; set; }
        //public Vendor VendorId { get; set; }
    }
}
