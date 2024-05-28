using EcommerceAPI.UnitOfWork.Trackable;

namespace EcommerceAPI.Data.Models
{
    public class InventoryItem : Entity
    {
        public int Id { get; set; }
        public DateTime InDate { get; set; }
        public DateTime OutDate { get; set; }
        public int OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
    }
}
