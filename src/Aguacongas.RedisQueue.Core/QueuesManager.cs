using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    public class QueuesManager : IManageQueues
    {
        private readonly ISubscriber _subscriber;
        private readonly IStore _store;
        private readonly IManageSubscription _subscriptionManager;
        public Task<IEnumerable<string>> Queues => throw new NotImplementedException();

        public async Task<Message> Dequeue(string fromQueueName)
        {
            var message = await _store.Pop(fromQueueName);
            return GetData(message);
        }

        public async Task<Guid> Enqueue(string address, string payload)
        {
            var id = Guid.NewGuid();
            var message = new Message
            {
                Created = DateTimeOffset.Now,
                QueueNane = GetQueueName(address),
                Payload = payload,
                Id = id,
            };
            await Enqueue(message).ConfigureAwait(false);
            return message.Id;
        }

        public async Task Enqueue(Message message)
        {
            await _store.Push(message).ConfigureAwait(false);
            await Publish(message.QueueNane, message.Id).ConfigureAwait(false);
        }

        public async Task<Message> Peek(string fromQueueName)
        {
            var message = await _store.Peek(fromQueueName).ConfigureAwait(false);
            return GetData(message);
        }


        public async Task<Message> Get(Guid id, string fromQueueName)
        {
            var message = await _store.Get(id, fromQueueName).ConfigureAwait(false);
            return GetData(message);
        }

        public async Task<long> GetCount(string queueName)
        {
            var keys = await _store.GetKeys(queueName);
            return keys.Count();
        }

        private static Message GetData(Message message)
        {
            if (message != null && message.Payload != null)
            {
                return message;
            }
            return null;
        }

        private async Task Publish(string address, Guid id)
        {
            if (!_subscriber.IsConnected(address))
            {
                _subscriber.Subscribe(address, async (c, v) =>
                {
                    await _subscriptionManager.Handle(c, v).ConfigureAwait(false);
                });
            }
            await _subscriber.PublishAsync(address, id.ToString());
        }

        private string GetQueueName(string address)
        {
            var index = address.LastIndexOf('/');
            return address.Substring(index == -1 ? 0 : index);
        }
    }
}
