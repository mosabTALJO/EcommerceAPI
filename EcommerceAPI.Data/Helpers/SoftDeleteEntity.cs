using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Data.Helpers
{
    public interface ISoftDelete
    {
        [Column(TypeName = "timestamp(6)")]
        DateTime? DeletedAt { get; set; }
        string? DeletedBy { get; set; }
        [NotMapped]
        public bool CompleteRemove { get; set; }
    }
    public class SoftDeleteEntity : IMyTrackable, ISoftDelete
    {
        [NotMapped]
        public bool CompleteRemove { get; set; }
        [NotMapped]
        public bool UpdateCreatedAt { get; set; }
        [Column(TypeName = "timestamp(6)")]
        public DateTime CreatedAt { get; set; }
        [MaxLength(40)]
        public string CreatedBy { get; set; }
        [Column(TypeName = "timestamp(6)")]
        public DateTime UpdatedAt { get; set; }
        [MaxLength(40)]
        public string UpdatedBy { get; set; }
        [Column(TypeName = "timestamp(6)")]
        public DateTime? DeletedAt { get; set; }
        [MaxLength(40)]
        public string? DeletedBy { get; set; }
    }
}