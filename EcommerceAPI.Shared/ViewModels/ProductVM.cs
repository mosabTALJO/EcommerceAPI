namespace EcommerceAPI.Shared.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }
        public required string NameAr { get; set; }
        public required string NameEn { get; set; }
        public required string NameTr { get; set; }
        public string FullDescriptionAr { get; set; } = string.Empty;
        public string FullDescriptionEn { get; set; } = string.Empty;
        public string FullDescriptionTr { get; set; } = string.Empty;
        public int? BrandId { get; set; }
        public BrandVM? Brand { get; set; }
        public int? CategoryId { get; set; }
        public CategoryVM? Category { get; set; }
        public double SalesPrice { get; set; }
        public double FinalSalesPrice => Math.Round(SalesPrice - SalesPrice * (Discount ?? 0) / 100, 1);
        public double? Discount { get; set; }
        public string MainImage { get; set; } = string.Empty;
        public List<string> Images { get; set; } = [];
        public string SKU { get; set; } = string.Empty;
        public string CompanyProductID { get; set; } = string.Empty;
        public string Weight { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsFavorite { get; set; }
        public bool OnlinePaymentOnly { get; set; }
        public ICollection<ProductVariantVM> ProductVariants { get; set; } = [];
    }
}
