using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class GiftPriceRange : TrackableEntity
    {
        public int Id { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public ICollection<Gift> Gifts { get; set; }
    }
}
