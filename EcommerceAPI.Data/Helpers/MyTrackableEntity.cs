using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Data.Helpers
{
    public interface IMyTrackable
    {
        [Column(TypeName = "timestamp(6)")]
        DateTime CreatedAt { get; set; }
        string CreatedBy { get; set; }
        [Column(TypeName = "timestamp(6)")]
        DateTime UpdatedAt { get; set; }
        string UpdatedBy { get; set; }
        [NotMapped]
        bool UpdateCreatedAt { get; set; }
    }

    public class TrackableEntity : IMyTrackable
    {
        [Column(TypeName = "timestamp(6)")]
        public DateTime CreatedAt { get; set; }
        [MaxLength(40)]
        public string CreatedBy { get; set; }
        [Column(TypeName = "timestamp(6)")]
        public DateTime UpdatedAt { get; set; }
        [MaxLength(40)]
        public string UpdatedBy { get; set; }
        [DefaultValue(false)]
        [NotMapped]
        public bool UpdateCreatedAt { get; set; }
    }
}
