using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;

namespace EcommerceAPI.Shared.ViewModels
{
    public class UserVM
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public double CustomerBalance { get; set; }
        public string Country { get; set; }
        public UserTypes UserType { get; set; }
        public List<Policies> Claims { get; set; }
        public bool IsBlackList { get; set; }
        public string IdCardImage { get; set; }
    }
}
