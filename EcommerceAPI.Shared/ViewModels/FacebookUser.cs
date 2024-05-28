using System.Text.Json.Serialization;

namespace EcommerceAPI.Shared.ViewModels
{
    public class FacebookUser
    {
        public string email { get; set; }
        [JsonPropertyName("picture.data.url")]
        public string picture { get; set; }
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }
}
