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
        /// Enqueues the specified to queue.
        /// </summary>
        /// <param name="address">
        /// Address of the queue. (https://host/queuename or queuename)
        /// </param>
        /// <param name="payload">
        /// The serialized message.
        /// </param>
        /// <returns>The message id</returns>
        Task<Guid> Enqueue(string address, string payload);

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
        Task Enqueue(Message message);

        /// <summary>
        ///   Dequeues the specified from queue.
        /// </summary>
        /// <param name="fromQueueName">
        ///   Name of from queue.
        /// </param>
        /// <returns>The first message in queue or null</returns>
        Task<Message> Dequeue(string fromQueueName);

        /// <summary>
        ///   Peeks the specified from queue.
        /// </summary>
        /// <param name="fromQueueName">
        ///   Name of queue queue.
        /// </param>
        /// <returns>
        ///   The first message in queue or null
        /// </returns>
        Task<Message> Peek(string fromQueueName);

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
        Task<Message> Get(Guid id, string fromQueueName);

        /// <summary>
        ///   Gets the number of message in a queue.
        /// </summary>
        /// <param name="queueName">
        ///   Name of the queue.
        /// </param>
        /// <returns>The number of message in a queue</returns>
        Task<long> GetCount(string queueName);

        /// <summary>
        ///   Gets the queues names.
        /// </summary>
        /// <value>
        ///   The queues names.
        /// </value>
        Task<IEnumerable<string>> Queues { get; }
    }
}
