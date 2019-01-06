using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    public class QueuesManager : IManageQueues
    {
        private readonly IStore _store;
        private readonly IManageSubscription _subscriptionManager;

        public QueuesManager(IStore store, IManageSubscription subscriptionManager)
        {
            _store = store;
            _subscriptionManager = subscriptionManager;
        }

        public IEnumerable<string> Queues => _subscriptionManager.Queues;

        public async Task<Message> DequeueAsync(string fromQueueName)
        {
            var message = await _store.PopAsync(fromQueueName);
            return GetData(message);
        }

        public async Task EnqueueAsync(Message message)
        {
            if (string.IsNullOrEmpty(message.QueueName))
            {
                throw new InvalidOperationException("The message queue name cannot be empty");
            }
            if (string.IsNullOrEmpty(message.Payload))
            {
                throw new InvalidOperationException("The message payload cannot be empty");
            }
            message.Created = DateTimeOffset.Now;
            await _store.PushAsync(message).ConfigureAwait(false);
            await Publish(message.QueueName, message.Id).ConfigureAwait(false);
        }

        public async Task<Message> PeekAsync(string fromQueueName)
        {
            var message = await _store.PeekAsync(fromQueueName).ConfigureAwait(false);
            return GetData(message);
        }


        public async Task<Message> GetAsync(Guid id, string fromQueueName)
        {
            var message = await _store.GetAsync(id, fromQueueName).ConfigureAwait(false);
            return GetData(message);
        }

        public async Task<long> GetCountAsync(string queueName)
        {
            var keys = await _store.GetKeysAsync(queueName);
            return keys.Count();
        }


        public Task<IEnumerable<Guid>> GetKeysAsync(string queueName) => _store.GetKeysAsync(queueName);

        private static Message GetData(Message message)
        {
            if (message != null && message.Payload != null)
            {
                message.InitiatorToken = null;
                return message;
            }
            return null;
        }

        private Task Publish(string address, Guid id)
        {
            return _subscriptionManager.Publish(address, id);
        }

        private string GetQueueName(string address)
        {
            var index = address.LastIndexOf('/');
            return address.Substring(index == -1 ? 0 : index);
        }
    }
}
