using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    /// <summary>
    /// Queues manager interface
    /// </summary>
    public interface IManageQueues
    {
        /// <summary>
        /// Enqueues the specified localy.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// For internal use, this method should not me call by your code
        /// </remarks>
        Task EnqueueAsync(Message message);

        /// <summary>
        ///   Dequeues the specified from queue.
        /// </summary>
        /// <param name="fromQueueName">
        ///   Name of from queue.
        /// </param>
        /// <returns>The first message in queue or null</returns>
        Task<Message> DequeueAsync(string fromQueueName);

        /// <summary>
        ///   Peeks the specified from queue.
        /// </summary>
        /// <param name="fromQueueName">
        ///   Name of queue queue.
        /// </param>
        /// <returns>
        ///   The first message in queue or null
        /// </returns>
        Task<Message> PeekAsync(string fromQueueName);
        /// <summary>
        /// Gets the keys asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetKeysAsync(string queueName);

        /// <summary>
        ///   Get the specified from queue.
        /// </summary>
        /// <param name="id">
        ///   The message id.
        /// </param>
        /// <param name="fromQueueName">
        ///   Name of queue.
        /// </param>
        /// <returns>
        ///   The data at id or null
        /// </returns>
        Task<Message> GetAsync(Guid id, string fromQueueName);

        /// <summary>
        ///   Gets the number of message in a queue.
        /// </summary>
        /// <param name="queueName">
        ///   Name of the queue.
        /// </param>
        /// <returns>The number of message in a queue</returns>
        Task<long> GetCountAsync(string queueName);
        void RebuildIndexesAndSubscribe();

        /// <summary>
        ///   Gets the queues names.
        /// </summary>
        /// <value>
        ///   The queues names.
        /// </value>
        IEnumerable<string> Queues { get; }
    }
}
