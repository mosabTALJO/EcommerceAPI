namespace EcommerceAPI.Shared.ViewModels
{
    public class CollectionItemVM
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public int ProductId { get; set; }
        public int OrderIndex { get; set; }
        public ProductLiteVM Product { get; set; }
    }
}
