namespace EcommerceAPI.Shared.ViewModels
{
    public class ProductVariantAttributeVM
    {
        public int ID { get; set; }
        public int ProductVariantID { get; set; }
        public int ProductAttributeID { get; set; }
        public required string Value { get; set; }
        public string AttributeName { get; set; }
    }
}
