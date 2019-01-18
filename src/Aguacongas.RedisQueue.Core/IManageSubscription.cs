using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    /// <summary>
    /// Subscription manager interface
    /// </summary>
    public interface IManageSubscription
    {
        /// <summary>
        /// Gets the queues names.
        /// </summary>
        /// <value>
        /// The queues.
        /// </value>
        IEnumerable<string> Queues { get; }
        /// <summary>
        /// Publishes a message has been queued.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task Publish(string address, Guid id);
        /// <summary>
        /// Subscribes to a queue events.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        void Subscribe(string queueName);
        /// <summary>
        /// Unsubscribes from a queue events.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        void Unsubscribe(string queueName);
    }
}
