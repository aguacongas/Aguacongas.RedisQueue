using Aguacongas.RedisQueue;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();

            var count = int.Parse(args[0]);

            for (int i = 0; i < count; i++)
            {
                client.PostAsync($"http://{args[1]}/api/queues/bench", new StringContent(JsonConvert.SerializeObject(i), Encoding.UTF8, "application/json"));
            }


        }
    }
}
