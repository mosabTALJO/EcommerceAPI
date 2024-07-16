using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Data.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }
        [Column(TypeName = "timestamp(6)")]
        public DateTime InDate { get; set; }
        [Column(TypeName = "timestamp(6)")]
        public DateTime OutDate { get; set; }
        public int OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
    }
}
