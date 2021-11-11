using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParticipantAPI.Repositories
{
    public interface IStorageRepository<T> where T : TableEntity, new()
    {
        Task Insert(T item);

        Task Update(T item);

        Task<IEnumerable<T>> GetAll();

        Task<T> GetItem(string partitionKey, string rowKey);
    }
}
