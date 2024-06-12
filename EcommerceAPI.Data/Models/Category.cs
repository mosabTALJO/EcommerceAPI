using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class Category : TrackableEntity
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
        public int? ParentId { get; set; }
        [JsonIgnore]
        public Category Parent { get; set; }
        public bool IsActive { get; set; }
        public int? OrderIndex { get; set; }
        public double? Discount { get; set; }
        public ICollection<Category> Children { get; set; }
        [NotMapped]
        public string HelperName { get; set; }

    }
}
