using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    public class Store : IStore
    {
        private readonly IDatabase _database;
        private readonly ISerialize _serilizer;

        public Store(IDatabase database, ISerialize serializer)
        {
            _database = database;
            _serilizer = serializer;
        }

        public Task<IEnumerable<string>> QueuesAsync()
        {
            var multipexer = _database.Multiplexer;
            var endpoints = multipexer.GetEndPoints();
            var queueNames = new List<string>();
            foreach(var enpoint in endpoints)
            {
                var server = multipexer.GetServer(enpoint);
                var cursor = server.Keys(_database.Database, "data/*");
                foreach(var key in cursor)
                {
                    var name = key.ToString().Substring(5);
                    if (!queueNames.Contains(name))
                    {
                        queueNames.Add(name);
                    }
                }
            }
            return Task.FromResult((IEnumerable<string>)queueNames);
        }

        public async Task PushAsync(Message message)
        {
            var transaction = _database.CreateTransaction();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            transaction.ListRightPushAsync("queue/" + message.QueueName, message.Id.ToString());
            transaction.HashSetAsync("data/" + message.QueueName, message.Id.ToString(), _serilizer.Serialize(message));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await transaction.ExecuteAsync();
        }

        public Task<Message> GetAsync(Guid id, string queueName)
        {
            return Get(queueName, id.ToString());
        }

        public async Task<IEnumerable<Guid>> GetKeysAsync(string queueName)
        {
            var ids = await _database.ListRangeAsync("queue/" + queueName);
            return GetIds(ids, queueName);
        }

        public async Task<Message> PopAsync(string fromQueueName)
        {
            string id = await _database.ListLeftPopAsync("queue/" + fromQueueName);
            var message = await Get(fromQueueName, id);
            if (message != null)
            {
                await _database.HashDeleteAsync("data/" + fromQueueName, id);
            }
            return message;
        }

        public async Task<Message> PeekAsync(string fromQueueName)
        {
            var id = await _database.ListGetByIndexAsync("queue/" + fromQueueName, 0);
            return await Get(fromQueueName, id);
        }


        public async Task<Message> PopIndexAsync(string fromQueueName)
        {
            string id = await _database.ListLeftPopAsync("queue/" + fromQueueName);
            return await Get(fromQueueName, id);
        }

        public Task RePushIndexAsync(Message message)
        {
            return _database.ListLeftPushAsync("queue/" + message.QueueName, message.Id.ToString());
        }

        public Task RemoveDataAsync(Message message)
        {
            return _database.HashDeleteAsync("data/" + message.QueueName, message.Id.ToString());
        }

        private async Task<Message> Get(string fromQueueName, string id)
        {
            if (id == null)
            {
                return null;
            }

            string message = await _database.HashGetAsync("data/" + fromQueueName, id);
            if (message != null)
            {
                return _serilizer.Deserialire(message);
            }
            return null;
        }

        private IEnumerable<Guid> GetIds(RedisValue[] ids, string queueName)
        {
            foreach(var id in ids)
            {
                if (_database.HashExists("data/" + queueName, id))
                {
                    yield return Guid.Parse(id);
                }
            }
        }
    }
}
