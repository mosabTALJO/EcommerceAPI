using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Trackable;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Data.Helpers
{
    public interface ISoftDelete
    {
        DateTime? DeletedAt { get; set; }
        string DeletedBy { get; set; }
        [NotMapped]
        public bool CompleteRemove { get; set; }
    }
    public class SoftDeleteEntity : Entity, ITrackable, ISoftDelete
    {
        [NotMapped]
        public bool CompleteRemove { get; set; }
        [NotMapped]
        public bool UpdateCreatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        [MaxLength(40)]
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        [MaxLength(40)]
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        [MaxLength(40)]
        public string DeletedBy { get; set; }
    }
}