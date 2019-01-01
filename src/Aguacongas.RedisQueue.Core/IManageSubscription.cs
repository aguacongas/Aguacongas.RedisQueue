using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    public interface IManageSubscription
    {
        Task Handle(RedisChannel channel, RedisValue value);
    }
}
