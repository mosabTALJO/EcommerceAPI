using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EcommerceAPI.Web.AddressScrapping
{
    public class AddressScrapper
    {
        static HttpClient client;
        static HttpClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new();
                    client.DefaultRequestHeaders.Add("Cookie", "__RequestVerificationToken=o_N1lMXeyBSkL1T7WoMKJ8QynI1DD4Q9I_QAlyxe96_82Rc3RIE5yfA0T_qSYe-n5MVjAbv6IULoq_00iSgNDTmFCEM1; TS01eef051=0179b2ce45cd917c2f7bf5043b700e9d602cfd35a1dc42285c945d9b9ee45d0005b3006da87bb6b497011ce292d1a364e2d5a631e5aff38f47f014c168b7733079ab167b55; browser-check=%22done%22");
                    client.DefaultRequestHeaders.Add("Referer", "https://adres.nvi.gov.tr/VatandasIslemleri/AdresSorgu");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    client.DefaultRequestHeaders.Add("__RequestVerificationToken", "SQI5Hch4J80l1Vo8c_P2mzLQ5QHSABnSG9p8d7QArEFPt-vyeUf8s6tCtnEhf6pcuJu50bTmuzg3mrq3VSqfG8uU-eM1");
                }
                return client;
            }
        }
        static string gaziantepDistrictsPath = $"{string.Join('\\', Directory.GetCurrentDirectory().Split('\\')[..4])}\\EcommerceAPI.Shared\\GaziantepDistricts.json";
        public static async Task AddDistricts(MyDbContext dbContext)
        {
            string jsonData = File.ReadAllText(gaziantepDistrictsPath);
            var ilces = JsonSerializer.Deserialize<IlceCollection>(jsonData);
            foreach (var ilce in ilces?.Property1 ?? [])
            {
                District district = new()
                {
                    CityId = ,
                    IsDelivery = false,
                    Name = ilce.bilesenAdi,
                    KimlikNo = ilce.kimlikNo,
                };
                dbContext.
            }
        }
        public static async Task ScrapNeighborhoods(int districtKimlikNo)
        {
            var body = new StringContent($"ilceKimlikNo={districtKimlikNo}&adresReCaptchaResponse=", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await Client.PostAsync("https://adres.nvi.gov.tr/Harita/mahalleKoyBaglisiListesi", body);
            var data = await response.Content.ReadAsStringAsync();
            var mahalles = JsonSerializer.Deserialize<MahalleCollection>(data);
            foreach (var mahalle in mahalles?.Property1 ?? [])
            {
                Neighborhood district = new()
                {
                    DistrictId = ,
                    IsDelivery = false,
                    Name = mahalle.bilesenAdi,
                };
            }
        }
    }
    public class MahalleCollection
    {
        public Mahalle[] Property1 { get; set; }
    }
    public class Mahalle
    {
        public string koyAdi { get; set; }
        public int koyKayitNo { get; set; }
        public int koyKurumBelediyeTur { get; set; }
        public int mahalleTur { get; set; }
        public string bilesenAdi { get; set; }
        public string adi { get; set; }
        public int kimlikNo { get; set; }
    }
    public class IlceCollection
    {
        public Ilce[] Property1 { get; set; }
    }
    public class Ilce
    {
        public int ilceKayitNo { get; set; }
        public string adi { get; set; }
        public int kimlikNo { get; set; }
        public string bilesenAdi { get; set; }
    }
}