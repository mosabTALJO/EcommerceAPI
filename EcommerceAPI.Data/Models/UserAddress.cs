using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class UserAddress : TrackableEntity
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        [MaxLength(200)]
        public string FirstName { get; set; }
        [MaxLength(200)]
        public string LastName { get; set; }
        [MaxLength(200)]
        public string Country { get; set; }
        [MaxLength(200)]
        public string City { get; set; }
        [MaxLength(2000)]
        public string FullAddress { get; set; }
        [MaxLength(30)]
        public string PhoneNumber { get; set; }
        public bool IsDefault { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
