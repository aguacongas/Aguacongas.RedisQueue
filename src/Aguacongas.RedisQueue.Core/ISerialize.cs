using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.RedisQueue
{
    public interface ISerialize
    {
        Message Deserialire(string value);

        string Serialize(Message message);
    }
}
