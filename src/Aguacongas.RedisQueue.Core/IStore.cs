using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    /// <summary>
    /// Store interface
    /// </summary>
    public interface IStore
    {
        /// <summary>
        /// Pushes the message asynchronousy to the queue.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        Task PushAsync(Message message);

        /// <summary>
        /// Gets a message from the queue asynchronously.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        Task<Message> GetAsync(Guid id, string queueName);

        /// <summary>
        /// Gets the queue keys asynchronously.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetKeysAsync(string queueName);

        /// <summary>
        /// Pops a message from the queues the asynchronously.
        /// </summary>
        /// <param name="fromQueueName">Name of from queue.</param>
        /// <returns></returns>
        Task<Message> PopAsync(string fromQueueName);
        /// <summary>
        /// Peeks a message from the queue asynchronously.
        /// </summary>
        /// <param name="fromQueueName">Name of from queue.</param>
        /// <returns></returns>
        Task<Message> PeekAsync(string fromQueueName);

        /// <summary>
        /// Pops a message index from the queue asynchronously.
        /// </summary>
        /// <param name="fromQueueName">Name of from queue.</param>
        /// <returns></returns>
        Task<Message> PopIndexAsync(string fromQueueName);

        /// <summary>
        /// Repush a message index to the queue asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        Task RepushIndexAsync(Message message);

        /// <summary>
        /// Removes the data asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        Task RemoveDataAsync(Message message);
        IEnumerable<string> GetQueueNameList();
        void RebuildIndex(string queueName);
    }
}
