using EcommerceAPI.UnitOfWork.Trackable;

namespace EcommerceAPI.Data.Models
{
    public class Collection : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CollectionItem> Items { get; set; }
    }
}
