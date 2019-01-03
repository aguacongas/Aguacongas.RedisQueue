using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    public interface IStore
    {
        Task Push(Message message);

        Task<Message> Get(Guid id, string queueName);
        Task<IEnumerable<Guid>> GetKeys(string queueName);
        Task<Message> Pop(string fromQueueName);
        Task<Message> Peek(string fromQueueName);
        
        Task<IEnumerable<string>> Queues();
    }
}
