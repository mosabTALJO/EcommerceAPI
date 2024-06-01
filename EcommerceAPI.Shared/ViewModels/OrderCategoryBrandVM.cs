namespace EcommerceAPI.Shared.ViewModels
{
    public class OrderCategoryBrandVM
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string BrandImage { get; set; }
        public int Count { get; set; }
        public int OrderIndex { get; set; }
        public int ProductsCount { get; set; }
    }
}