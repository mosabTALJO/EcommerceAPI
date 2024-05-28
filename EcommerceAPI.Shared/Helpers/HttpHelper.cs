using System.Net.Http.Headers;
using System.Text;
using System.Diagnostics;

namespace EcommerceAPI.Shared.Helpers
{
    public class HttpHelper
    {
        private static readonly HttpClient client = new HttpClient();
        private static async Task<HttpResponseMessage> PostToServer(string url, Dictionary<string, string> values, string Token = null, string BasicAuth = null)
        {
            try
            {
                HttpContent content = null;
                if (values != null)
                    content = new FormUrlEncodedContent(values);
                if (BasicAuth != null)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(BasicAuth))); // XAppUser:^77Fb$=mK#~)
                else if (Token != null)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var response = await client.PostAsync(url, content);
                return response;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }
        private static async Task<HttpResponseMessage> PostToServer(string url, object o, string Token = null, string BasicAuth = null)
        {
            try
            {
                var content = System.Text.Json.JsonSerializer.Serialize(o);
                if (BasicAuth != null)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", BasicAuth); // XAppUser:^77Fb$=mK#~)
                else if (Token != null)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                var stringContent = new StringContent(content,
                         Encoding.UTF8,
                         "application/json");

                var response = await client.PostAsync(url, stringContent);

                return response;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }
        private static async Task<HttpResponseMessage> PutToServer(string url, object o, string Token = null, string BasicAuth = null)
        {
            try
            {
                var content = System.Text.Json.JsonSerializer.Serialize(o);
                if (BasicAuth != null)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(BasicAuth))); // XAppUser:^77Fb$=mK#~)
                else if (Token != null)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(url, stringContent);

                return response;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }
        private static async Task<HttpResponseMessage> DeleteFromServer(string url, object o, string Token = null, string BasicAuth = null)
        {
            try
            {
                if (BasicAuth != null)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(BasicAuth))); // XAppUser:^77Fb$=mK#~)
                else if (Token != null)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                var response = await client.DeleteAsync(url);
                return response;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }
        public static async Task<HttpResponseMessage> GetFromServer(string url, string Token = null, string BasicAuth = null)
        {
            try
            {
                if (BasicAuth != null)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(BasicAuth))); // XAppUser:^77Fb$=mK#~)
                else if (Token != null)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var response = await client.GetAsync(url);

                return response;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }

        public async static Task<T> GetDataFromResponse<T>(HttpResponseMessage response)
        {
            try
            {
                if (response is null)
                    return default;

                string json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(json);
                return System.Text.Json.JsonSerializer.Deserialize<T>(json.ToString());
            }
            catch (Exception ex)
            {
                LogException(ex);
                return default;
            }
        }

        public async static Task<T> Get<T>(string url, string Token = null, string BasicAuth = null)
        {
            try
            {
                Debug.WriteLine("GET: URL: " + url);
                var r = await GetFromServer(url, Token, BasicAuth);
                var ar = await GetDataFromResponse<T>(r);
                if (r != null && r.StatusCode == System.Net.HttpStatusCode.OK)
                    return ar;
                throw new Exception(await r?.Content?.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return default;
        }
        public async static Task<T> Post<T>(string url, object o, string Token = null, string BasicAuth = null)
        {
            try
            {
                var r = await PostToServer(url, o, Token, BasicAuth);
                var ar = await GetDataFromResponse<T>(r);
                if (r != null && r.StatusCode == System.Net.HttpStatusCode.OK)
                    return ar;
                throw new Exception(await r?.Content?.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return default;
        }
        public async static Task<T> Post<T>(string url, Dictionary<string, string> values, string Token = null, string BasicAuth = null)
        {
            try
            {
                var r = await PostToServer(url, values, Token, BasicAuth);
                var ar = await GetDataFromResponse<T>(r);
                if (r?.StatusCode == System.Net.HttpStatusCode.OK)
                    return ar;

                throw new Exception(await r?.Content?.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return default;
        }
        public async static Task<T> Put<T>(string url, object o, string Token = null, string BasicAuth = null)
        {
            try
            {
                var r = await PutToServer(url, o, Token, BasicAuth);
                var ar = await GetDataFromResponse<T>(r);
                if (r != null && r.StatusCode == System.Net.HttpStatusCode.OK)
                    return ar;

                throw new Exception(await r?.Content?.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return default;
        }
        public async static Task<T> Put<T>(string url, Dictionary<string, string> values, string Token = null, string BasicAuth = null)
        {
            try
            {
                var r = await PutToServer(url, values, Token, BasicAuth);
                var ar = await GetDataFromResponse<T>(r);

                if (r != null && r.StatusCode == System.Net.HttpStatusCode.OK)
                    return ar;

                throw new Exception(await r?.Content?.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return default;
        }
        public async static Task<T> Delete<T>(string url, object o, string Token = null, string BasicAuth = null)
        {
            try
            {
                var r = await DeleteFromServer(url, o, Token, BasicAuth);
                var ar = await GetDataFromResponse<T>(r);

                if (r != null && r.StatusCode == System.Net.HttpStatusCode.OK)
                    return ar;

                throw new Exception(await r?.Content?.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return default;
        }

        public static void LogException(Exception ex)
        {
            throw ex;
        }
    }
}
