using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class Rating : TrackableEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int? ProductVariantID { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        public double Rate { get; set; }
        public string Description { get; set; }
        public RatingType RatingType { get; set; }
    }

    public enum RatingType
    {
        Service,
        Brand,
        Product
    }
}
