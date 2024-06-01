namespace EcommerceAPI.Shared.ViewModels
{
    public class CollectionVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOneFree { get; set; }
        public CollectionItemVM[] Items { get; set; }
    }
}
