using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using AzureCostDashboard.azfn.services;
using azfn;

namespace AzureCostDashboardFunction
{
    public class InsertCostEntries
    {
        private readonly ILogger<InsertCostEntries> _logger;
        private readonly string _cosmosEndpointUri = Environment.GetEnvironmentVariable("COSMOS_ENDPOINT_URI");
        private readonly string _cosmosPrimaryKey = Environment.GetEnvironmentVariable("COSMOS_PRIMARY_KEY");
        private readonly string _cosmosDatabaseName = Environment.GetEnvironmentVariable("COSMOS_DATABASE_NAME");
        private readonly string _cosmosContainerName = Environment.GetEnvironmentVariable("COSMOS_CONTAINER_NAME");
        private readonly string _cosmosPartitionKey = Environment.GetEnvironmentVariable("COSMOS_PARTITION_KEY");

        private CosmosDBService<AzureCostEntry> cosmosService;

        public InsertCostEntries(ILogger<InsertCostEntries> logger)
        {
            _logger = logger;
        }

        [Function("InsertCostEntries")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var response = req.CreateResponse(HttpStatusCode.OK);

            // Get JSON from the request body
            string requestBody = await new System.IO.StreamReader(req.Body).ReadToEndAsync();
            List<AzureCostEntry> data = System.Text.Json.JsonSerializer.Deserialize<List<AzureCostEntry>>(requestBody);

            cosmosService = new CosmosDBService<AzureCostEntry>(_cosmosEndpointUri, _cosmosPrimaryKey, _cosmosDatabaseName);
            // await cosmosService.EnsureDeleteContainerAsync(_cosmosContainerName);  // Uncomment if deleting the container is needed
            await cosmosService.EnsureCreateContainerAsync(_cosmosContainerName, _cosmosPartitionKey);

            // Write to Cosmos DB
            var insertResponse = new InsertCostEntryResponse();
            insertResponse.RecordCount = data.Count;
            var tasks = new List<Task>();
            data.ForEach(item => {
                item.Id ??= CreateBase64StringFromValues(new List<string> { item.SubscriptionGuid, item.ResourceGuid, item.Date.ToString() });
                tasks.Add(cosmosService.CreateItemAsync(item));
            });

            Task task = null;
            try
            {
                task = Task.WhenAll(tasks);
                await task;
            }
            catch
            {
                var exceptions = task.Exception;
                foreach (var exception in exceptions.InnerExceptions)
                {
                    var id = exception.Data.Contains("Id") ? $" Id:{exception.Data["Id"]}" : "";
                    insertResponse.Exceptions.Add(exception.Message + id);
                }
            }

            await response.WriteAsJsonAsync(insertResponse);
            return response;
        }

        private string CreateBase64StringFromValues(List<string> values)
        {
            // Create a base64 encoded hash from the values
            var hash = string.Join("", values);
            var bytes = System.Text.Encoding.UTF8.GetBytes(hash);
            var encoded = Convert.ToBase64String(bytes);
            return encoded;
        }

    }
}
