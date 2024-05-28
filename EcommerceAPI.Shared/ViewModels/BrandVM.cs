namespace EcommerceAPI.Shared.ViewModels
{
    public class BrandVM
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string NameTr { get; set; }
        public string Image { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }
        public int OrderIndex { get; set; }
        public int ProductsCount { get; set; }
    }
}
