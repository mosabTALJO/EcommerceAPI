using EcommerceAPI.UnitOfWork.Trackable;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Data.Models
{
    public class District : Entity
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public bool IsDelivery { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
    }
}
