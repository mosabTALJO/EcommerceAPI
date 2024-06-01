using EcommerceAPI.UnitOfWork.Trackable;

namespace EcommerceAPI.Data.Models
{
    public class Settings : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
