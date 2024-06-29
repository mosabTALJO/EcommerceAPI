namespace EcommerceAPI.Shared.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string NameTr { get; set; }
        public string FullDescriptionAr { get; set; }
        public string FullDescriptionEn { get; set; }
        public string FullDescriptionTr { get; set; }
        public int BrandId { get; set; }
        public BrandVM Brand { get; set; }
        public int? CategoryId { get; set; }
        public CategoryVM Category { get; set; }
        public double SalesPrice { get; set; }
        public double FinalSalesPrice => Math.Round(SalesPrice - SalesPrice * (Discount ?? 0) / 100, 1);
        public double? Discount { get; set; }
        public string MainImage { get; set; }
        public List<string> Images { get; set; }
        public string SKU { get; set; }
        public string CompanyProductID { get; set; }
        public string Weight { get; set; }
        public bool IsActive { get; set; }
        public bool IsFavorite { get; set; }
        public ICollection<ProductVariantVM> ProductVariants { get; set; }
        public bool OnlinePaymentOnly { get; set; }
    }
}
