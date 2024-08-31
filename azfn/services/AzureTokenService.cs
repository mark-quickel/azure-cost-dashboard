namespace AzureCostDashboardFunction.Services
{
    public class AzureTokenService
    {
        private readonly HTTPService _httpService;

        public AzureTokenService()
        {
            _httpService = new HTTPService();
        }

        // Get an access token scoped to the Azure Management API
        public string GetAccessToken()
        {
            var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
            var resourceId = Environment.GetEnvironmentVariable("RESOURCE_ID");
            var endpoint = Environment.GetEnvironmentVariable("AZURE_AUTHORIZE_ENDPOINT");

            var body = $"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}&resource={resourceId}"; 
            var response = _httpService.MakeRequest(HttpMethod.Post,endpoint,null,body,"application/x-www-form-urlencoded");
            var token = _httpService.GetResponseByKey(response,"access_token");
            return token;
        }
 
    }
}