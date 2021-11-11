using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using ParticipantAPI.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParticipantAPI.Repositories
{
    public class AzureTableStorage<T> : IStorageRepository<T> where T : TableEntity, new()
    {
        private readonly AzureTableSettings _settings;
        public AzureTableStorage(AzureTableSettings settings)
        {
            _settings = settings;
        }

        private async Task<CloudTable> GetTableAsync()
        {
            //Account  
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new StorageCredentials(_settings.StorageAccount, _settings.StorageKey), false);

            //Client  
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            //Table  
            CloudTable table = tableClient.GetTableReference(_settings.TableName);
            await table.CreateIfNotExistsAsync();

            return table;
        }

        public async Task Insert(T item)
        {
            //Table  
            CloudTable table = await GetTableAsync();

            //Operation  
            TableOperation operation = TableOperation.Insert(item);

            //Execute  
            await table.ExecuteAsync(operation);
        }

        public async Task Update(T item)
        {
            //Table  
            CloudTable table = await GetTableAsync();

            //Operation  
            TableOperation operation = TableOperation.InsertOrReplace(item);

            //Execute  
            await table.ExecuteAsync(operation);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            //Table  
            CloudTable table = await GetTableAsync();

            //Query  
            TableQuery<T> query = new TableQuery<T>();

            List<T> results = new List<T>();
            TableContinuationToken continuationToken = null;
            do
            {
                TableQuerySegment<T> queryResults =
                    await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results);

            } while (continuationToken != null);

            return results;
        }

        public async Task<T> GetItem(string partitionKey, string rowKey)
        {
            //Table  
            CloudTable table = await GetTableAsync();

            //Operation  
            TableOperation operation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            //Execute  
            TableResult result = await table.ExecuteAsync(operation);

            return (T)result.Result;
            
        }
    }
}
