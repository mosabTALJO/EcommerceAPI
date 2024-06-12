using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace EcommerceAPI.Core.UserAddresses.Queries
{
    public class FindUserAddressQuery : IRequest<RequestResult<UserAddressVM>>
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string UserId { get; set; }
        public class FindUserAddressQueryHandler : IRequestHandler<FindUserAddressQuery, RequestResult<UserAddressVM>>
        {
            private readonly UserManager<AppUser> _userManager;
            public FindUserAddressQueryHandler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }
            public async Task<RequestResult<UserAddressVM>> Handle(FindUserAddressQuery query, CancellationToken cancellationToken)
            {
                if (query.UserId == null) return new RequestResult<UserAddressVM>(ActionResultType.NotFound, "id not found");
                var u = await _userManager.FindByIdAsync(query.UserId);
                if (u == null) return new RequestResult<UserAddressVM>(ActionResultType.NotFound, "user not found");

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                httpClient.DefaultRequestHeaders.Add("Referer", "http://www.microsoft.com");
                var jsonData = await httpClient.GetAsync("http://nominatim.openstreetmap.org/reverse?format=json&lat=" + query.Lat + "&lon=" + query.Lon);
                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(await jsonData.Content.ReadAsStringAsync());

                return new RequestResult<UserAddressVM>(new UserAddressVM
                {
                    AppUserId = query.UserId,
                    Lat = query.Lat,
                    Lng = query.Lon,
                    City = rootObject.address.city,
                    Country = rootObject.address.country_code,
                    Name = u.DisplayName,
                    PhoneNumber = u.PhoneNumber,
                    FullAddress = rootObject.display_name,
                });
            }
        }
    }

    [DataContract]
    public class Address
    {
        [DataMember]
        public string road { get; set; }
        [DataMember]
        public string suburb { get; set; }
        [DataMember]
        public string city { get; set; }
        [DataMember]
        public string state_district { get; set; }
        [DataMember]
        public string state { get; set; }
        [DataMember]
        public string postcode { get; set; }
        [DataMember]
        public string country { get; set; }
        [DataMember]
        public string country_code { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember]
        public string place_id { get; set; }
        [DataMember]
        public string licence { get; set; }
        [DataMember]
        public string osm_type { get; set; }
        [DataMember]
        public string osm_id { get; set; }
        [DataMember]
        public string lat { get; set; }
        [DataMember]
        public string lon { get; set; }
        [DataMember]
        public string display_name { get; set; }
        [DataMember]
        public Address address { get; set; }
    }
}
