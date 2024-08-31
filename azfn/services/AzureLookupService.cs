#nullable enable

namespace AzureCostDashboardFunction.Services
{
    public class AzureLookupService
    {
        private readonly HTTPService _httpService;
        private readonly string _accessToken;

        public AzureLookupService(string accessToken)
        {
            _accessToken = accessToken;
            _httpService = new HTTPService();
        }

        public dynamic Request(HttpMethod method, string endpoint, string accessToken, string responseKey="value", string? body = null, string contentType = "application/json")
        {
            var response = _httpService.MakeRequest(method,endpoint,accessToken,body,contentType);
            var value = _httpService.GetResponseByKey(response,responseKey);
            return value;
        }

    }
}