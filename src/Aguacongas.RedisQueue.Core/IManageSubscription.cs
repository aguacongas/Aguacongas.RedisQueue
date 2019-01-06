using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    public interface IManageSubscription
    {
        IEnumerable<string> Queues { get; }

        Task Handle(RedisChannel channel, RedisValue value);
        Task Publish(string address, Guid id);
    }
}
