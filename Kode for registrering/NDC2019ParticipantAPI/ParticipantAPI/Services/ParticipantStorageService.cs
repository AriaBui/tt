using Microsoft.WindowsAzure.Storage;
using ParticipantAPI.Models;
using ParticipantAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParticipantAPI.Services
{
    public class ParticipantStorageService : IParticipantStorageService
    {
        private readonly IStorageRepository<Participant> _repository;

        public ParticipantStorageService(IStorageRepository<Participant> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Add(Participant item)
        {
            try
            {
                var existingItem = await _repository.GetItem(item.PartitionKey, item.RowKey);
                if (existingItem == null)
                {
                    await _repository.Insert(item);
                }
                else
                {
                    if (item.Score > existingItem.Score)
                    {
                        await _repository.Update(item);
                    }
                }
            }
            catch (StorageException)
            {
                return false;
            }

            return true;
        }

        public async Task<Participant> Get(string partitionKey, string rowKey)
        {
            try
            {
                return await _repository.GetItem(partitionKey, rowKey);
            }
            catch (StorageException)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Participant>> GetAll()
        {
            try
            {
                return await _repository.GetAll();
            }
            catch (StorageException)
            {
                return null;
            }
        }

        public async Task<bool> Update(Participant item)
        {
            try
            {
                await _repository.Update(item);
            }
            catch (StorageException)
            {
                return false;
            }
            return true;
        }
    }
}
