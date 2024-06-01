namespace EcommerceAPI.Shared.ViewModels
{
    public class InitDataModel
    {
        public string VersionNumber { get; set; } = "1.22";
        public BrandVM[] Brands { get; set; }
        public List<CategoryVM> Categories { get; set; }
        public CurrencyRateLiteVM[] CurrencyRates { get; set; }
        public object[] Colors { get; set; }
        public object[] Slides { get; set; }
        public string[] Sizes { get; set; }
        public string error { get; set; }
        public List<int> WomenCategoryIds { get; set; }
        public List<string> FirebaseTopics { get; set; }
        public UserVM? User { get; set; }
    }
    public class InitData
    {
        public string VersionNumber { get; set; } = "1.22";
        public List<CategoryVM> Categories { get; set; }
        public List<CountryVM> Countries { get; set; }
        public List<BaseItemVM> BaseItems { get; set; }
        public List<int> WomenCategoryIds { get; set; }
        public List<string> FirebaseTopics { get; set; }
        public UserVM? User { get; set; }
    }
    public class WebInitData
    {
        public List<CategoryVM> Categories { get; set; }
        public List<CountryVM> Countries { get; set; }
        public List<BaseItemVM> BaseItems { get; set; }
    }
    public class InitDataAdminModel
    {
        public BrandVM[] Brands { get; set; }
        public List<CategoryLite> Categories { get; set; }
        public object[] Colors { get; set; }
    }
}
