using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Data.Models
{
    public class ProductVariantAttribute
    {
        [Key]
        public int ID { get; set; }
        public int ProductVariantID { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        public int ProductAttributeID { get; set; }
        public ProductAttribute? ProductAttribute { get; set; }
        public required string Value { get; set; }
    }
}