using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Data.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        [MaxLength(200)]
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public UserTypes UserType { get; set; }
        public string NotificationID { get; set; } = string.Empty;
        public string FacebookId { get; set; } = string.Empty;
        public string GoogleId { get; set; } = string.Empty;
        public string AppleId { get; set; } = string.Empty;
        [MaxLength(200)]
        public string Country { get; set; } = string.Empty;
        public ICollection<UserAddress> UserAddresses { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
    public enum UserTypes
    {
        Admin,
        Customer,
        NotRegisteredCustomer,
    }
}
