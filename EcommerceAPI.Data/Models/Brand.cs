using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class Brand : TrackableEntity
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public string NameAr { get; set; }
        [MaxLength(200)]
        public string NameEn { get; set; }
        [MaxLength(200)]
        public string NameTr { get; set; }
        [MaxLength(60)]
        public string Image { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }
        public int? OrderIndex { get; set; }
    }
}