using EcommerceAPI.Data.Helpers;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Data.Models
{
    public class ProductAttribute : SoftDeleteEntity
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(200)]
        public required string NameAr { get; set; }
        [MaxLength(200)]
        public required string NameTr { get; set; }
        [MaxLength(200)]
        public required string NameEn { get; set; }
    }
}