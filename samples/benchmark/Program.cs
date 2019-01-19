using Aguacongas.RedisQueue;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace benchmark
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string receiverhost = args[0];
            int index = 1;
            if (args.Length == 3)
            {
                receiverhost = args[1];
                index = 2;
            }

            var connection = new HubConnectionBuilder()
                .WithUrl($"http://{receiverhost}/queues")
                .Build();

            await connection.StartAsync();

            await connection.InvokeAsync("SubscribeToQueue", "bench");

            var count = int.Parse(args[index]);
            var initialCount = count;
            var evt = new ManualResetEvent(false);
            var client = new HttpClient();
            connection.On<string>("newMessage", async id =>
            {
                HttpResponseMessage getresponse;
                int result;
                do
                {
                    getresponse = await client.GetAsync($"http://{receiverhost}/api/queues/pop/bench");
                    result = Interlocked.Decrement(ref count);
                } while (getresponse.StatusCode == HttpStatusCode.OK);
                if (result <= 0)
                {
                    evt.Set();
                }
            });

            Console.WriteLine("Clear queue");

            HttpResponseMessage response;
            do
            {
                response = await client.GetAsync($"http://{receiverhost}/api/queues/pop/bench");
            } while (response.StatusCode == HttpStatusCode.OK);

            Console.WriteLine("Start sending");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < count; i++)
            {
                var content = JsonConvert.SerializeObject(new
                {
                    Index = i,
                    Value = $"A bigger oject for {i}"
                });
                client.PostAsync($"http://{args[0]}/api/queues/{Uri.EscapeUriString($"http://{receiverhost}/api/queues/bench")}", new StringContent($"{content}", Encoding.UTF8, "application/json"));
            }

            evt.WaitOne();
            stopwatch.Stop();
            
            Console.WriteLine(stopwatch.Elapsed.ToString());
            Console.WriteLine($"{(double)initialCount / (double)(stopwatch.ElapsedMilliseconds / 1000)}/s");
        }
    }
}
