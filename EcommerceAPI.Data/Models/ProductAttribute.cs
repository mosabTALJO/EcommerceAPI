using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Data.Models
{
    public class ProductAttribute
    {
        [Key]
        public int ID { get; set; }
        public required string Name { get; set; }
        public List<ProductVariantAttribute> ProductVariantAttributes { get; set; } = [];
    }
}