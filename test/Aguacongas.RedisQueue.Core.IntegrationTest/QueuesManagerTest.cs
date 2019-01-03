using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.RedisQueue.Core.IntegrationTest
{
    [Collection("Redis")]
    public class QueuesManagerTest
    {
        private readonly TestFixture _fixture;

        public QueuesManagerTest(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task EnQueuePeekGetDequeuCountTest()
        {
            var sut = GetSut();

            var message = new Message
            {

            };
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.EnqueueAsync(message));

            message.QueueName = Guid.NewGuid().ToString();

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.EnqueueAsync(message));

            message.Payload = Guid.NewGuid().ToString();

            await sut.EnqueueAsync(message);

            Assert.Equal(1, await sut.GetCountAsync(message.QueueName));

            var messageRead = await sut.GetAsync(message.Id, message.QueueName);

            Assert.Equal(messageRead.Payload, message.Payload);

            messageRead = await sut.PeekAsync(message.QueueName);

            Assert.Equal(messageRead.Id, message.Id);
            Assert.Equal(messageRead.Payload, message.Payload);

            messageRead = await sut.DequeueAsync(message.QueueName);

            Assert.Equal(messageRead.Id, message.Id);
            Assert.Equal(messageRead.Payload, message.Payload);

            Assert.Equal(0, await sut.GetCountAsync(message.QueueName));
        }

        [Fact]
        public async Task RemoteTest()
        {
            var waitHandle = new AutoResetEvent(false);

            var status = 500;
            _fixture.OnMessageRecieved = context =>
            {
                context.Response.StatusCode = status;

                new Timer(state =>
                {
                    waitHandle.Set();
                }, null, 1000, Timeout.Infinite);

                return Task.CompletedTask;
            };

            var sut = GetSut();

            var message = new Message
            {
                Payload = Guid.NewGuid().ToString(),
                QueueName = _fixture.TestServer.BaseAddress.ToString()
            };

            await sut.EnqueueAsync(message);

            waitHandle.WaitOne();
            waitHandle.WaitOne();

            var count = await sut.GetCountAsync(message.QueueName);

            Assert.Equal(1, count);

            status = 200;

            waitHandle.WaitOne();
            waitHandle.WaitOne();

            count = await sut.GetCountAsync(message.QueueName);

            Assert.Equal(0, count);
        }



        private IManageQueues GetSut()
        {
            return _fixture.Provider.GetRequiredService<IManageQueues>();
        }
    }
}
