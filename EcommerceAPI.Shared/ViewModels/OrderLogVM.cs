using EcommerceAPI.Data.Models;

namespace EcommerceAPI.Shared.ViewModels
{
    public class OrderLogVM
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? OrderItemId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public OrderItemState? OrderItemState { get; set; }
        public UserTypes UserType { get; set; }
        public string Username { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
