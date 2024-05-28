using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class Coupon : SoftDeleteEntity
    {
        public int Id { get; set; }

        [MaxLength(40)]
        public string Code { get; set; }
        [MaxLength(200)]
        public string NameAr { get; set; }
        [MaxLength(200)]
        public string NameEn { get; set; }
        [MaxLength(200)]
        public string NameTr { get; set; }
        public DateTime ExpireDate { get; set; }
        public int Count { get; set; }
        public int RemainingCount { get; set; }
        public double Rate { get; set; }
        public bool IsActive { get; set; }
        public CouponType Type { get; set; }
        public int? TypeId { get; set; }
        public string? CustomerId { get; set; }
        public bool IsForCustomerFirstOrder { get; set; }
    }
    public enum CouponType { General, Product, Category, Brand, }
}