using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class FrontendSetting : TrackableEntity
    {
        public int Id { get; set; }
        public FrontendSettingType Type { get; set; }
        public string TextTr { get; set; } = string.Empty;
        public string TextEn { get; set; } = string.Empty;
        public string TextAr { get; set; } = string.Empty;
    }

    public enum FrontendSettingType
    {
        PrivacyPolicy = 1,
        FrequentlyAskedQuestions = 2,
        HowCanIReturnIt = 3,
        WhoAreWe = 4,
    }
}
