using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.RedisQueue.Extensions
{
    /// <summary>
    /// Message extensions
    /// </summary>
    public static class MessageExtension
    {
        /// <summary>
        /// To the dto.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="initiatorToken">The initiator token.</param>
        /// <returns></returns>
        public static Message ToDto(this Model.Message message, string initiatorToken)
        {
            return new Message
            {
                Content = message.Content,
                Created = message.Created,
                Id = message.Id,
                QueueName = message.QueueName,
                InitiatorToken = initiatorToken
            };
        }

        /// <summary>
        /// To the model.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static Model.Message ToModel(this Message message)
        {
            return new Model.Message
            {
                Content = message.Content,
                Created = message.Created,
                Id = message.Id,
                QueueName = message.QueueName
            };
        }
    }
}
