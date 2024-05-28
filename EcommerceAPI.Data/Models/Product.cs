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
        public string NameAr { get; set; }
        [MaxLength(200)]
        public string NameEn { get; set; }
        [MaxLength(200)]
        public string NameTr { get; set; }
        public string FullDescriptionAr { get; set; }
        public string FullDescriptionEn { get; set; }
        public string FullDescriptionTr { get; set; }
        public int? BrandId { get; set; }
        public Brand Brand { get; set; }
        //public Vendor VendorId { get; set; }
        public double SalesPrice { get; set; }
        public double? Discount { get; set; }
        [MaxLength(400)]
        public string MainImage { get; set; }
        [MaxLength(5000)]
        public string Images { get; set; }
        [MaxLength(50)]
        public string SKU { get; set; }
        [MaxLength(50)]
        public string CompanyProductID { get; set; }
        [MaxLength(500)]
        public string CompanyURL { get; set; }
        public double? TotalRate { get; set; }
        [MaxLength(50)]
        public string Weight { get; set; }
        public bool IsActive { get; set; } = true;
        public ProductTypes ProductType { get; set; }
        public double? CompanyPrice { get; set; }
        [MaxLength(255)]
        public string CompanyBrand { get; set; }
        public string CompanyColor { get; set; }
        public int TranslationFlag { get; set; }
        public int? GenderId { get; set; }
        public DateTime? CompanyUpdatedDate { get; set; }
        public ICollection<ProductType> ProductTypes { get; set; }
        public bool OnlinePaymentOnly { get; set; }
    }

    //public enum Vendor
    //{
    //    Trendyol = 1,
    //    LCWaikiki = 3,
    //    Defacto = 4,
    //    Zara = 5,
    //    InStreet = 6,
    //    Mavi = 7,
    //    Flo = 8,
    //    Koton = 9,
    //    Custom = 10,
    //}
    public enum ProductTypes
    {
        Single,
        Matrix
    }
}