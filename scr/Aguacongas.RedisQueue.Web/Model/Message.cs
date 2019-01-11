using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aguacongas.RedisQueue.Model
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
        [Required]
        public string QueueName { get; set; }
        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public DateTimeOffset Created { get; set; }
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [Required]
        public string Content { get; set; }
    }
}
