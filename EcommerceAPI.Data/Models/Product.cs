using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class Product : SoftDeleteEntity
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        [MaxLength(200)]
        public string? NameAr { get; set; }
        [MaxLength(200)]
        public string NameEn { get; set; }
        [MaxLength(200)]
        public string? NameTr { get; set; }
        public string? FullDescriptionAr { get; set; }
        public string? FullDescriptionEn { get; set; }
        public string? FullDescriptionTr { get; set; }
        public int? BrandId { get; set; }
        public Brand Brand { get; set; }
        [MaxLength(400)]
        public string MainImage { get; set; }
        [MaxLength(5000)]
        public string Images { get; set; }
        [MaxLength(50)]
        public string SKU { get; set; }
        public double? TotalRate { get; set; }
        public bool IsActive { get; set; } = true;
        public double SalesPrice { get; set; }
        public double SalesPriceUSD { get; set; }
        public double? DefaultCostPrice { get; set; }
        public double? Discount { get; set; }
        public ICollection<ProductVariant>? ProductVariants { get; set; }
        public bool OnlinePaymentOnly { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = string.Empty;
    }
}