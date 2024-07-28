using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Data.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public string FullDescription { get; set; }
        [MaxLength(100)]
        public string? Barcode { get; set; }
        public int StockLevel { get; set; }
        public double SalesPrice { get; set; }
        public double? Discount { get; set; }
        [MaxLength(5000)]
        public string Images { get; set; }
        [MaxLength(50)]
        public string SKU { get; set; }
        public bool IsActive { get; set; } = true;
        public List<ProductVariantAttribute> Attributes { get; set; } = [];
    }
}
