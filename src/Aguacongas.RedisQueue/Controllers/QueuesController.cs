using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.RedisQueue.Controllers
{
    /// <summary>
    /// Manage queues
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("queues/")]
    [Produces("application/json")]
    [ApiController]
    public class QeuesController : ControllerBase
    {
        private readonly IManageQueues _manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="QeuesController"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public QeuesController(IManageQueues manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Gets all queues.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return _manager.Queues;
        }

        /// <summary>
        /// Dequeue a message.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        [HttpGet("{queueName}")]
        public async Task<Message> Get(string queueName)
        {
            return await _manager.DequeueAsync(queueName);
        }

        /// <summary>
        /// Peeks the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        [HttpGet("{queueName}/peek")]
        public async Task<Message> Peek(string queueName)
        {
            return await _manager.PeekAsync(queueName);
        }

        /// <summary>
        /// Reads the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet("{queueName}/{id}")]
        public async Task<Message> Read(string queueName, Guid id)
        {
            return await _manager.GetAsync(id, queueName);
        }

        /// <summary>
        /// Counts the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        [HttpGet("{queueName}/count")]
        public async Task<long> Count(string queueName)
        {
            return await _manager.GetCountAsync(queueName);
        }

        /// <summary>
        /// Keyses the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        [HttpGet("{queueName}/keys")]
        public async Task<IEnumerable<Guid>> Keys(string queueName)
        {
            return await _manager.GetKeysAsync(queueName);
        }

        /// <summary>
        /// Posts the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [HttpPost("{*destination}")]
        public async Task<Guid> Post([FromRoute] string destination, [FromBody] string value)
        {
            destination = SanetizeDestination(destination);

            var message = new Message
            {
                Payload = value,
                QueueName = destination,
                InitiatorToken = HttpContext.Request.Headers
                    .Where(h => h.Key == "Authorization")
                    .Select(h => h.Value).FirstOrDefault()
            };

            await _manager.EnqueueAsync(message);

            return message.Id;
        }

        /// <summary>
        /// Puts the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        [HttpPut("{*destination}")]
        public async Task Put([FromRoute] string destination, [FromBody] Message message)
        {
            destination = SanetizeDestination(destination);

            message.QueueName = destination;
            await _manager.EnqueueAsync(message);
        }

        private static string SanetizeDestination(string destination)
        {
            if (SubscriptionManager.IsRemote.IsMatch(destination))
            {
                destination = destination.Replace(":/", "://");
            }

            return destination;
        }
    }
}
