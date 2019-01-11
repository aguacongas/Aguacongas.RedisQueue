using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    /// <summary>
    /// Manage queues
    /// </summary>
    /// <seealso cref="Aguacongas.RedisQueue.IManageQueues" />
    public class QueuesManager : IManageQueues
    {
        private readonly IStore _store;
        private readonly IManageSubscription _subscriptionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuesManager"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="subscriptionManager">The subscription manager.</param>
        public QueuesManager(IStore store, IManageSubscription subscriptionManager)
        {
            _store = store;
            _subscriptionManager = subscriptionManager;
        }

        /// <summary>
        /// Gets the queues names.
        /// </summary>
        /// <value>
        /// The queues names.
        /// </value>
        public IEnumerable<string> Queues => _subscriptionManager.Queues;

        /// <summary>
        /// Dequeues messages from queue.
        /// </summary>
        /// <param name="fromQueueName">Name of from queue.</param>
        /// <returns>
        /// The first message in queue or null
        /// </returns>
        public async Task<Message> DequeueAsync(string fromQueueName)
        {
            var message = await _store.PopAsync(fromQueueName);
            return GetData(message);
        }

        /// <summary>
        /// Enqueues messages localy.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        /// The message queue name cannot be empty
        /// or
        /// The message payload cannot be empty
        /// </exception>
        /// <remarks>
        /// For internal use, this method should not me call by your code
        /// </remarks>
        public async Task EnqueueAsync(Message message)
        {
            if (string.IsNullOrEmpty(message.QueueName))
            {
                throw new InvalidOperationException("The message queue name cannot be empty");
            }
            if (string.IsNullOrEmpty(message.Content))
            {
                throw new InvalidOperationException("The message payload cannot be empty");
            }
            message.Created = DateTimeOffset.Now;
            await _store.PushAsync(message).ConfigureAwait(false);
            await Publish(message.QueueName, message.Id).ConfigureAwait(false);
        }

        /// <summary>
        /// Peeks a message from queue.
        /// </summary>
        /// <param name="fromQueueName">Name of queue queue.</param>
        /// <returns>
        /// The first message in queue or null
        /// </returns>
        public async Task<Message> PeekAsync(string fromQueueName)
        {
            var message = await _store.PeekAsync(fromQueueName).ConfigureAwait(false);
            return GetData(message);
        }


        /// <summary>
        /// Get a message from queue.
        /// </summary>
        /// <param name="id">The message id.</param>
        /// <param name="fromQueueName">Name of queue.</param>
        /// <returns>
        /// The data at id or null
        /// </returns>
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


        /// <summary>
        /// Gets the keys asynchronously.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        public Task<IEnumerable<Guid>> GetKeysAsync(string queueName) => _store.GetKeysAsync(queueName);

        private static Message GetData(Message message)
        {
            if (message != null && message.Content != null)
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
