using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Data.Helpers
{
    public interface IMyTrackable
    {
        DateTime CreatedAt { get; set; }
        string CreatedBy { get; set; }
        DateTime UpdatedAt { get; set; }
        string UpdatedBy { get; set; }
        [NotMapped]
        bool UpdateCreatedAt { get; set; }
    }

    public class TrackableEntity : IMyTrackable
    {
        public DateTime CreatedAt { get; set; }
        [MaxLength(40)]
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        [MaxLength(40)]
        public string UpdatedBy { get; set; }
        [DefaultValue(false)]
        [NotMapped]
        public bool UpdateCreatedAt { get; set; }
    }
}
