namespace EcommerceAPI.Shared.ViewModels
{
    public class GiftPriceRangeVM
    {
        public int Id { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public int CountryId { get; set; }
        public CountryVM Country { get; set; }
        public ICollection<GiftVM> Gifts { get; set; }
    }
}
