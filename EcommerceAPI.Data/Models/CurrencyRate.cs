using EcommerceAPI.UnitOfWork.Trackable;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Data.Models
{
    public class CurrencyRate : Entity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string CountryCode { get; set; }
        public CurrencyRateTypes Type { get; set; }
        public double Rate { get; set; }
        public double FeeRate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }

        [NotMapped]
        public double FianlRate => FeeRate * Rate;
    }
    public enum CurrencyRateTypes
    {
        KWD = 1,
        USD,
        SAR,
        AED,
        OMR,
        QAR,
        EGP,
        IQD,
        MAD,
        JOD,
        DZD,
        BHD,
    }
}
