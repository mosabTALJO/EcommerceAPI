using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class FrontendSetting : TrackableEntity
    {
        public int Id { get; set; }
        public FrontendSettingType Type { get; set; }
        public string TextEn { get; set; }
        public string TextAr { get; set; }
    }

    public enum FrontendSettingType
    {
        PrivacyPolicy = 1,
        FrequentlyAskedQuestions = 2,
        HowCanIReturnIt = 3,
        WhoAreWe = 4,
    }
}
