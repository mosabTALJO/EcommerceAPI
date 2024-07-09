using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class UserAddress : TrackableEntity
    {
        public int Id { get; set; }
        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; }
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(200)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(200)]
        public string LastName { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }

        [MaxLength(2000)]
        public string FullAddress { get; set; } = string.Empty;
        [MaxLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
