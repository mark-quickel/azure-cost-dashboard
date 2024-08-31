#nullable enable
using System.Dynamic;
using Newtonsoft.Json;

namespace AzureCostDashboardFunction.Services
{
    public class HTTPService
    {
        public string MakeRequest(HttpMethod method, string? url, string? accessToken = null, string? body = null, string contentType = "application/json")
        {
            if (url is null)
                return String.Empty;

            var request = new HttpRequestMessage(method, url);
            if (accessToken is not null)
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
            if (body is not null)
            {
                request.Content = new StringContent(body); 
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            }

            var httpClient = new HttpClient();
            var response = httpClient.SendAsync(request).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            return responseContent;
        }

        public dynamic GetResponseByKey(string response, string key)
        {
            var responseJson = JsonConvert.DeserializeObject<ExpandoObject>(response);
            var value = responseJson.FirstOrDefault(x => x.Key == key).Value;
            return value;
        }
       
    }
}