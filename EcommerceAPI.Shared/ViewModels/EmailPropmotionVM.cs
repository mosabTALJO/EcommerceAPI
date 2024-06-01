using EcommerceAPI.Data.Models;

namespace EcommerceAPI.Shared.ViewModels
{
    public class EmailPromotionLiteVM
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public PromotionFilter Filter { get; set; }
        public DateTime? FilterDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class EmailPromotionVM
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public PromotionFilter Filter { get; set; }
        public DateTime? FilterDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string HtmlText { get; set; }
    }
}
