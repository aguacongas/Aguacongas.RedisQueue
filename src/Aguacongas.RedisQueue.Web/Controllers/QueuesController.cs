﻿using Aguacongas.RedisQueue.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aguacongas.RedisQueue.Controllers
{
    /// <summary>
    /// Manage queues
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Authorize(Policy = "RedisQueues")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class QueuesController : ControllerBase
    {
        private static Regex _isUri { get; } = new Regex("^(http|https)://");
        private readonly IManageQueues _manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuesController" /> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public QueuesController(IManageQueues manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Gets all queues names.
        /// </summary>
        /// <returns>List of queue name</returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return _manager.Queues;
        }

        /// <summary>
        /// Dequeue a message.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>The 1st message in the queue</returns>
        [HttpGet("pop/{*queueName}")]
        public async Task<Model.Message> Get(string queueName)
        {
            queueName = SanetizeDestination(queueName);
            return (await _manager.DequeueAsync(queueName)).ToModel();
        }

        /// <summary>
        /// Peeks the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>The 1st message in the queue</returns>
        [HttpGet("peek/{*queueName}")]
        public async Task<Model.Message> Peek(string queueName)
        {
            return (await _manager.PeekAsync(SanetizeDestination(queueName))).ToModel();
        }

        /// <summary>
        /// Reads the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>A message</returns>
        [HttpGet("get/{id}/{*queueName}")]
        public async Task<Model.Message> Read(string queueName, Guid id)
        {
            return (await _manager.GetAsync(id, SanetizeDestination(queueName))).ToModel();
        }

        /// <summary>
        /// Counts the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        [HttpGet("count/{*queueName}")]
        public async Task<long> Count(string queueName)
        {
            return await _manager.GetCountAsync(SanetizeDestination(queueName));
        }

        /// <summary>
        /// Gets the identifier list.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>The list of message identifier of specified queue</returns>
        [HttpGet("ids/{*queueName}")]
        public async Task<IEnumerable<Guid>> GetIdList(string queueName)
        {
            return await _manager.GetKeysAsync(SanetizeDestination(queueName));
        }

        /// <summary>
        /// Creates a new message with the serialized value and queue it to the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="content">The serialized content.</param>
        /// <returns>The message identifier</returns>
        [HttpPost("{*destination}")]
        public async Task<Guid> Post([FromRoute] string destination, [FromBody][Required] object content)
        {
            destination = SanetizeDestination(destination);
            var message = new Message
            {
                Content = content,
                QueueName = destination,
                InitiatorToken = GetInitiatorToken()
            };

            await _manager.EnqueueAsync(message);

            return message.Id;
        }

        /// <summary>
        /// Queues a message to the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        [HttpPut("{*destination}")]
        public async Task Put([FromRoute] string destination, [FromBody][Required] Model.Message message)
        {
            message.QueueName = SanetizeDestination(destination);
            await _manager.EnqueueAsync(message.ToDto(GetInitiatorToken()));
        }

        /// <summary>
        /// Deletes the specified queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        [HttpDelete("{*queueName}")]
        public async Task Delete([FromRoute] string queueName)
        {
            await _manager.DeleteQueueAsync(SanetizeDestination(queueName));
        }

        private static string SanetizeDestination(string destination)
        {
            destination = Uri.UnescapeDataString(destination);
            if (!_isUri.IsMatch(destination) && SubscriptionManager.IsRemote.IsMatch(destination))
            {
                destination = destination.Replace(":/", "://");
            }

            return destination;
        }

        private StringValues GetInitiatorToken()
        {
            return HttpContext.Request.Headers
                                .Where(h => h.Key == "Authorization")
                                .Select(h => h.Value).FirstOrDefault();
        }

    }
}
