
using System.Dynamic;
using Newtonsoft.Json.Linq;

namespace AzureCostDashboardFunction.Services
{
    public class ResourceService
    {
        private readonly AzureLookupService _azureService;
        private string _accessToken;
        private readonly static string _endpoint = Environment.GetEnvironmentVariable("AZURE_RESORUCES_ENDPOINT");

        public ResourceService(string accessToken)
        {
            _azureService = new AzureLookupService(_accessToken = accessToken);
        }

        public dynamic Get()
        {
            return _azureService.Request(HttpMethod.Get, _endpoint,_accessToken);
        }
    }
}