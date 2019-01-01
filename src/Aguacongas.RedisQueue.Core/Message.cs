using System;

namespace Aguacongas.RedisQueue
{
    public class Message
    {
        public Guid Id { get; set; }

        public string QueueNane { get; set; }
        public DateTimeOffset Created { get; set; }

        public string InitiatorToken { get; set; }
        public string Payload{ get; set; }
    }
}
