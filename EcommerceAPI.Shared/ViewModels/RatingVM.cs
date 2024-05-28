namespace EcommerceAPI.Shared.ViewModels
{
    public class RatingVM
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        //public OrderVM Order { get; set; }
        public int? RatedItemId { get; set; }
        public string? RatedItemName { get; set; }
        public string? RatedItemPicture { get; set; }
        public double Rate { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerId { get; set; }
        public string RatingType { get; set; }
    }
}
