using EcommerceAPI.Data.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Data.Models
{
    public class EmailPromotion : TrackableEntity
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public PromotionFilter Filter { get; set; }
        public DateTime? FilterDate { get; set; }
        public string HtmlText { get; set; }
    }

    public enum PromotionFilter
    {
        AllCustomers = 0,
        RealCustomers = 1,
        ActiveCustomersSince = 2
    }
}
