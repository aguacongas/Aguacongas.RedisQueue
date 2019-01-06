using System;

namespace Aguacongas.RedisQueue
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string QueueName { get; set; }
        public DateTimeOffset Created { get; set; }
        public string InitiatorToken { get; set; }
        public string Payload{ get; set; }
    }   
}
