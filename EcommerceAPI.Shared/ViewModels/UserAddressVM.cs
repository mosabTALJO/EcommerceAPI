namespace EcommerceAPI.Shared.ViewModels
{
    public class UserAddressVM
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string FullAddress { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDefault { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
