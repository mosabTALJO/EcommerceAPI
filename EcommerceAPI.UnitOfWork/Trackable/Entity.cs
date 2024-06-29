using EcommerceAPI.UnitOfWork.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.UnitOfWork.Trackable
{
    public abstract class Entity : IMergeable
    {
        [NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }

        [NotMapped]
        public Guid EntityIdentifier { get; set; }
    }

}
