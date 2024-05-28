using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Data.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(200)]
        public string CreatedUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserTypes UserType { get; set; }
        public double CustomerBalance { get; set; }
        public string NotificationID { get; set; }
        public string FacebookId { get; set; }
        public string GoogleId { get; set; }
        public string AppleId { get; set; }
        //public byte? Gender { get; set; }//0 male, 1 female
        //public DateTime? DOB { get; set; }
        [MaxLength(200)]
        public string Country { get; set; }
        public ICollection<UserAddress> UserAddresses { get; set; }
        public ICollection<Order> Orders { get; set; }
        public bool IsBlackList { get; set; }
        public string IdCardImage { get; set; }
    }
    public enum UserTypes
    {
        Admin,
        Customer,
        NotRegisteredCustomer,
    }
}
