using ParticipantAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParticipantAPI.Services
{
    public interface IParticipantStorageService
    {
        Task<bool> Add(Participant item);

        Task<bool> Update(Participant item);

        Task<IEnumerable<Participant>> GetAll();

        Task<Participant> Get(string partitionKey, string rowKey);
    }
}
