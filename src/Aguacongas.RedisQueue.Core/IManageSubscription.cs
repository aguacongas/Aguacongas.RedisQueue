using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    public interface IManageSubscription
    {
        Task Handle(RedisChannel channel, RedisValue value);
        Task Publish(string address, Guid id);
    }
}
