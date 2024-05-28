namespace EcommerceAPI.Shared.ViewModels
{
    public class CategoryOrdering
    {
        public int CategoryId { get; set; }
        public int OrderIndex { get; set; }
        public double ProfitRatio { get; set; }
        public double Discount { get; set; }
        public double DefaultWeight { get; set; }
        public List<CategoryOrdering> Children { get; set; }
    }
}
