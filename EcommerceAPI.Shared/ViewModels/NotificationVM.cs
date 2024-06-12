namespace EcommerceAPI.Shared.ViewModels
{
    public class NotificationVM
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Topic { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }

        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? CollectionId { get; set; }
        public int? ProductId { get; set; }
        public int? OrderId { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
