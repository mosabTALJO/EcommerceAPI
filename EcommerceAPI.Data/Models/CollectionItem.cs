namespace EcommerceAPI.Data.Models
{
    public class CollectionItem
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int OrderIndex { get; set; }
    }
}
