using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    /// <summary>
    /// Store messages to queues
    /// </summary>
    /// <seealso cref="Aguacongas.RedisQueue.IStore" />
    public class Store : IStore
    {
        private readonly IDatabase _database;
        private readonly ISerialize _serilizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Store"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="serializer">The serializer.</param>
        public Store(IDatabase database, ISerialize serializer)
        {
            _database = database;
            _serilizer = serializer;
        }

        /// <summary>
        /// Pushes the message asynchronousy to the queue.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public async Task PushAsync(Message message)
        {
            var transaction = _database.CreateTransaction();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            transaction.ListRightPushAsync("queue/" + message.QueueName, message.Id.ToString());
            transaction.HashSetAsync("data/" + message.QueueName, message.Id.ToString(), _serilizer.Serialize(message));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await transaction.ExecuteAsync();
        }

        /// <summary>
        /// Gets a message from the queue asynchronously.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        public Task<Message> GetAsync(Guid id, string queueName)
        {
            return Get(queueName, id.ToString());
        }

        /// <summary>
        /// Gets the queue keys asynchronously.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Guid>> GetKeysAsync(string queueName)
        {
            var ids = await _database.ListRangeAsync("queue/" + queueName);
            return GetIds(ids, queueName);
        }

        /// <summary>
        /// Pops a message from the queues the asynchronously.
        /// </summary>
        /// <param name="fromQueueName">Name of from queue.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Peeks a message from the queue asynchronously.
        /// </summary>
        /// <param name="fromQueueName">Name of from queue.</param>
        /// <returns></returns>
        public async Task<Message> PeekAsync(string fromQueueName)
        {
            var id = await _database.ListGetByIndexAsync("queue/" + fromQueueName, 0);
            return await Get(fromQueueName, id);
        }


        /// <summary>
        /// Pops a message index from the queue asynchronously.
        /// </summary>
        /// <param name="fromQueueName">Name of from queue.</param>
        /// <returns></returns>
        public async Task<Message> PopIndexAsync(string fromQueueName)
        {
            string id = await _database.ListLeftPopAsync("queue/" + fromQueueName);
            return await Get(fromQueueName, id);
        }

        /// <summary>
        /// Repush a message index to the queue asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public Task RepushIndexAsync(Message message)
        {
            return _database.ListLeftPushAsync("queue/" + message.QueueName, message.Id.ToString());
        }

        /// <summary>
        /// Removes the data asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
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
