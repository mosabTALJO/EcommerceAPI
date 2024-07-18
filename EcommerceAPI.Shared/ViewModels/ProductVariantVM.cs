using EcommerceAPI.Data.Models;

namespace EcommerceAPI.Shared.ViewModels
{
    public class ProductVariantVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string FullDescription { get; set; }
        public string? Barcode { get; set; }
        public int StockLevel { get; set; }
        public double SalesPrice { get; set; }
        public double? Discount { get; set; }
        public double FinalSalesPrice => Math.Round(SalesPrice - SalesPrice * (Discount ?? 0) / 100, 1);
        public string MainImage { get; set; }
        public List<string> Images { get; set; } = [];
        public string SKU { get; set; }
        public bool IsActive { get; set; } = true;
        public List<ProductVariantAttributeVM> Attributes { get; set; } = [];
    }
}
