namespace EcommerceAPI.Shared.ViewModels
{
    public class ProductTypeVM
    {
        //calculated
        public int Id { get; set; }
        public int StockLevel { get; set; }
        public string Size { get; set; }
        public string MainImage { get; set; }
        public string SKU { get; set; }
        public string SizeAr { get; set; }
        public string SizeEn { get; set; }
        public string Barcode { get; set; }
    }
}