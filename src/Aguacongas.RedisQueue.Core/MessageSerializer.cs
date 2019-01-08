using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.RedisQueue
{
    /// <summary>
    /// Serialize messages
    /// </summary>
    /// <seealso cref="Aguacongas.RedisQueue.ISerialize" />
    public class MessageSerializer : ISerialize
    {
        /// <summary>
        /// Gets or sets the json serializer settings.
        /// </summary>
        /// <value>
        /// The json serializer settings.
        /// </value>
        public static JsonSerializerSettings JsonSerializerSettings { get; } = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.None,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        /// <summary>
        /// Deserialires the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Message Deserialire(string value)
        {
            return JsonConvert.DeserializeObject<Message>(value, JsonSerializerSettings);
        }

        /// <summary>
        /// Serializes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public string Serialize(Message message)
        {
            return JsonConvert.SerializeObject(message, JsonSerializerSettings);
        }
    }
}
