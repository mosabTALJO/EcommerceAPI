using EcommerceAPI.UnitOfWork.Trackable;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Data.Models
{
    public class City : Entity
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string NameAr { get; set; }
        [MaxLength(200)]
        public string NameEn { get; set; }
        [MaxLength(200)]
        public string NameTr { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }
        
        public int CountryId { get; set; }
        public Country Country { get; set; }
    }
}
