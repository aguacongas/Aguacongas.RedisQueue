using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.RedisQueue.Core.IntegrationTest
{
    public class TestFixture
    {
        public IDatabase Database { get; }
        public ServiceProvider Provider { get; }
        public TestServer TestServer { get; }

        public Func<HttpContext, Task> OnMessageRecieved { get; set; }

        public TestFixture()
        {
            var options = new ConfigurationOptions();
            options.EndPoints.Add("localhost:6379");
            options.AllowAdmin = true;

            var multiplexer = ConnectionMultiplexer.Connect(options);
            Database = multiplexer.GetDatabase();
            var server = multiplexer.GetServer("localhost:6379");
            server.FlushDatabase();

            var services = new ServiceCollection();
            services.AddRedisQueue("localhost:6379");

            var webHostBuilder = new WebHostBuilder();
            webHostBuilder.Configure(app =>
            {
                app.Use((context, next) =>
                {
                    return OnMessageRecieved?.Invoke(context);
                });
            });
            TestServer = new TestServer(webHostBuilder);
            services.AddTransient(provider => TestServer.CreateClient());

            Provider = services.BuildServiceProvider();
        }
    }

    [CollectionDefinition("Redis")]
    public class CollectionFixture : ICollectionFixture<TestFixture>
    {
    }
}
