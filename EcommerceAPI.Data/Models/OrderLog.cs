using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class OrderLog : TrackableEntity
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public int? OrderId { get; set; }
        public OrderItem OrderItem { get; set; }
        public int? OrderItemId { get; set; } = null;
        public OrderStatus OrderStatus { get; set; }
        public OrderItemState? OrderItemState { get; set; } = null;
        public UserTypes UserType { get; set; }
        public string Username { get; set; }
        public string Notes { get; set; }
    }
}
