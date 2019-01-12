using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue
{
    /// <summary>
    /// SignalR queue hub
    /// </summary>
    /// <seealso cref="Hub" />
    public class QueueHub : Hub
    {
        /// <summary>
        /// Subscribes to queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        public async Task SubscribeToQueue(string queueName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, queueName);
        }

        /// <summary>
        /// Unsubscribes from queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        public async Task UnsubscribeFromQueue(string queueName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, queueName);
        }
    }
}
