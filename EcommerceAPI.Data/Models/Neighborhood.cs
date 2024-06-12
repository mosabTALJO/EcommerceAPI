using EcommerceAPI.UnitOfWork.Trackable;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Data.Models
{
    public class Neighborhood : Entity
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public bool IsDelivery { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; }
    }
}
