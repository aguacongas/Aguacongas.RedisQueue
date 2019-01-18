using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aguacongas.RedisQueue;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace standalone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BenchController : ControllerBase
    {
        private readonly IManageQueues _manager;

        public BenchController(IManageQueues manager)
        {
            _manager = manager;
        }

        [HttpPut]
        public void Put(int count)
        {
            for(int i = 0; i < count; i++)
            {
                var message = new Message
                {
                    Content = JsonConvert.SerializeObject(i),
                    Created = DateTimeOffset.Now,
                    QueueName = $"{Request.Scheme}://{Request.Host}/api/queues/bench"
                };
                _manager.EnqueueAsync(message);
            }
        }
    }
}