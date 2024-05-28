namespace EcommerceAPI.Shared.ViewModels
{
    public class CouponLiteVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string NameTr { get; set; }
        public DateTime ExpireDate { get; set; }
        public double Rate { get; set; }
        public bool IsForCustomerFirstOrder { get; set; }
    }
}
