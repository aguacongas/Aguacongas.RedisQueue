using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.RedisQueue
{
    /// <summary>
    /// Message serializer interface
    /// </summary>
    public interface ISerialize
    {
        /// <summary>
        /// Deserialires the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        Message Deserialire(string value);

        /// <summary>
        /// Serializes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        string Serialize(Message message);
    }
}
