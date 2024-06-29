using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Data.Models
{
    public class ProductFavorite
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
