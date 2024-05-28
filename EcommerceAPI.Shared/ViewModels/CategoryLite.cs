namespace EcommerceAPI.Shared.ViewModels
{
    public class CategoryLite
    {
        public CategoryLite()
        {
        }
        public CategoryLite(int id, string name, int? parentId)
        {
            Id = id;
            Name = name;
            ParentId = parentId;
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
    }
}
