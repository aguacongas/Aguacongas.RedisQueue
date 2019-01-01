using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.RedisQueue
{
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

        public Message Deserialire(string value)
        {
            return JsonConvert.DeserializeObject<Message>(value, JsonSerializerSettings);
        }

        public string Serialize(Message message)
        {
            return JsonConvert.SerializeObject(message, JsonSerializerSettings);
        }
    }
}
