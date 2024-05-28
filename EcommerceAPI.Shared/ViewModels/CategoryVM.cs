namespace EcommerceAPI.Shared.ViewModels
{
    public class CategoryVM
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string NameTr { get; set; }
        public string Image { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public int? OrderIndex { get; set; }
        public string FullName { get; set; }
        public double? ProfitRatio { get; set; }
        public double? Discount { get; set; }
        public double? DefaultWeight { get; set; }
        public ICollection<CategoryVM> Children { get; set; }
    }
}
