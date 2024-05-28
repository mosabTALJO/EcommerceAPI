using EcommerceAPI.UnitOfWork.Trackable;

namespace EcommerceAPI.Data.Models
{
    public class Inventory : Entity
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAR { get; set; }
        public ICollection<InventoryItem> InventoryItems { get; set; }
    }
}
