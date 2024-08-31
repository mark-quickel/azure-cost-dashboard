using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using AzureCostDashboardFunction.Services;
using AzureCostDashboard.azfn.services;
using Newtonsoft.Json;

namespace AzureCostDashboardFunction
{
    // Add service principal to Reader role under subscription
    public class GetBillingAccounts
    {
        private readonly ILogger _logger;
        private readonly string _cosmosEndpointUri = Environment.GetEnvironmentVariable("COSMOS_ENDPOINT_URI");
        private readonly string _cosmosPrimaryKey = Environment.GetEnvironmentVariable("COSMOS_PRIMARY_KEY");
        private readonly string _cosmosDatabaseName = Environment.GetEnvironmentVariable("COSMOS_DATABASE_NAME");
        private readonly string _cosmosContainerName = Environment.GetEnvironmentVariable("COSMOS_CONTAINER_NAME");
        private readonly string _cosmosPartitionKey = Environment.GetEnvironmentVariable("COSMOS_PARTITION_KEY");
        private CosmosDBService<AzureCostEntry> _dbService;

        public GetBillingAccounts(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetBillingAccounts>();
            _dbService = new CosmosDBService<AzureCostEntry>(_cosmosEndpointUri, _cosmosPrimaryKey, _cosmosDatabaseName);
        } 

        [Function("GetBillingAccounts")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var response = req.CreateResponse(HttpStatusCode.OK);

          //  await _dbService.InitializeAsync();
            
            var token = new AzureTokenService().GetAccessToken();
            //var subscriptions = new SubscriptionService(token).Get();
           // await _dbService.UpsertItemAsync(subscriptions);
            // write to cosmos
            //var resourcegroups = new ResourceGroupService(token).Get();
            // write to cosmos
            var resources = new ResourceService(token).Get();
            // write to cosmos
           // var billingaccounts = new BillingAccountService(token).Get();
            // write to cosmos

            var output = JsonConvert.SerializeObject(resources);

            await response.WriteAsJsonAsync("ASDF");
            return response;
        }

    }
}

