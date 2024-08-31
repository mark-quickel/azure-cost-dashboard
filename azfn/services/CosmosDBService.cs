using AzureCostDashboardFunction;
using Microsoft.Azure.Cosmos;

namespace AzureCostDashboard.azfn.services
{
    public class CosmosDBService<T> where T : AzureCostEntry
    {
        public Exception NotFoundException;
        public Exception ConflictException;
        private readonly string _endpointUri;
        private readonly string _primaryKey;
        private readonly string _databaseName;

        private CosmosClient _cosmosClient;
        private Database _database;
        private Microsoft.Azure.Cosmos.Container _container;

        public CosmosDBService(string endpointUri, string primaryKey, string databaseName)
        {
            _endpointUri = endpointUri;
            _primaryKey = primaryKey;
            _databaseName = databaseName;

            _cosmosClient = new CosmosClient(_endpointUri, _primaryKey, new CosmosClientOptions() { AllowBulkExecution = true });
            _database = _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName).Result;
        }

        public async Task EnsureCreateContainerAsync(string name, string partitionKey)
        {
            _container = await _database.CreateContainerIfNotExistsAsync(name, partitionKey);
        }

        public async Task EnsureDeleteContainerAsync(string name)
        {
            await _database.GetContainer(name).DeleteContainerAsync();
        }

        public async Task CreateItemAsync(T item)
        {
            try
            {
                 await _container.CreateItemAsync<T>(item, new PartitionKey(item.SubscriptionGuid));
            }
            catch (CosmosException ex)
            {
                ex.Data.Add("Id", item.Id);
                throw;
            }
        }
    
        public async Task<T> GetByIdAsync(T item)
        {
            ItemResponse<T> response = null;

            try
            {
                 response = await _container.ReadItemAsync<T>(item.Id, new PartitionKey(item.SubscriptionGuid));
            }
            catch (CosmosException ex)
            {
                ex.Data.Add("Id", item.Id);
                throw;
            }

            return response.Resource;
        }

        public async Task QueryItemsAsync()
        {
            var sqlQueryText = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<dynamic> queryResultSetIterator = _container.GetItemQueryIterator<dynamic>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<dynamic> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (var item in currentResultSet)
                {
                    Console.WriteLine(item);
                }
            }
        }

    }

}