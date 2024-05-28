using EcommerceAPI.Data.Models;

namespace EcommerceAPI.Shared.ViewModels
{
    public class CouponVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string NameTr { get; set; }
        public DateTime ExpireDate { get; set; }
        public int Count { get; set; }
        public int RemainingCount { get; set; }
        public double Rate { get; set; }
        public bool IsActive { get; set; }
        public CouponType Type { get; set; }
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public int CustomersCount { get; set; }
        public int PaidCustomersCount { get; set; }
        public double TotalSales { get; set; }
        public string TypeText => Type == CouponType.General ? "General" :
            Type == CouponType.Brand ? "Brand" : Type == CouponType.Category ? "Category" : "Product";

        public bool IsForCustomerFirstOrder { get; set; }
    }
}
