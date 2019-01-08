using System;

namespace Aguacongas.RedisQueue
{
    /// <summary>
    /// Define a message
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        /// <value>
        /// The name of the queue.
        /// </value>
        public string QueueName { get; set; }
        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public DateTimeOffset Created { get; set; }
        /// <summary>
        /// Gets or sets the initiator authorization token.
        /// </summary>
        /// <value>
        /// The initiator token.
        /// </value>
        public string InitiatorToken { get; set; }
        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        /// <value>
        /// The payload.
        /// </value>
        public string Payload{ get; set; }
    }   
}
