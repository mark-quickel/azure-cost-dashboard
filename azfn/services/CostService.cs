
using System.Dynamic;
using Newtonsoft.Json.Linq;

namespace AzureCostDashboardFunction.Services
{
    public class CostService 
    {
        private readonly AzureLookupService _azureService;
        private string _accessToken;
        private readonly static string _endpoint = Environment.GetEnvironmentVariable("AZURE_COSTS_ENDPOINT");

        public CostService(string accessToken)
        {
            _azureService = new AzureLookupService(_accessToken = accessToken);
        }

        public dynamic Get()
        {
            var body = $"{{\"type\":\"Usage\",\"timeframe\":\"MonthToDate\",\"dataset\":{{\"granularity\":\"Daily\"}}}}}}";
            return _azureService.Request(HttpMethod.Post, _endpoint,_accessToken, body, "application/json");
        }
    }
}