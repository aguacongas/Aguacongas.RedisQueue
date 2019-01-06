using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    public interface IStore
    {
        Task PushAsync(Message message);

        Task<Message> GetAsync(Guid id, string queueName);
        Task<IEnumerable<Guid>> GetKeysAsync(string queueName);
        Task<Message> PopAsync(string fromQueueName);
        Task<Message> PeekAsync(string fromQueueName);
        
        Task<IEnumerable<string>> QueuesAsync();
        Task<Message> PopIndexAsync(string fromQueueName);
        Task RePushIndexAsync(Message message);
        Task RemoveDataAsync(Message message);
    }
}
