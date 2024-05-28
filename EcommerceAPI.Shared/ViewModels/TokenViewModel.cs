using EcommerceAPI.Shared.Helpers;

namespace EcommerceAPI.Shared.ViewModels
{
    public class TokenViewModel
    {
        public string userID { get; set; }
        public string token { get; set; }
        public DateTime expiration { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Country { get; set; }
        public List<Policies> Claims { get; set; }
    }
}