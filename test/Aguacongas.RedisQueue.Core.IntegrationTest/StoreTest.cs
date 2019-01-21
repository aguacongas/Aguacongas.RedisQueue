using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.RedisQueue.Core.IntegrationTest
{
    [Collection("Redis")]
    public class StoreTest
    {
        private readonly TestFixture _fixture;

        public StoreTest(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task PopIndexAsync_should_check_if_message_exists()
        {
            var queue = Guid.NewGuid().ToString();
            var message1 = new Message
            {
                Content = "test",
                QueueName = queue
            };
            var message2 = new Message
            {
                Content = "test",
                QueueName = queue
            };

            var serializer = new MessageSerializer();

            var sut = new Store(_fixture.Database, serializer);

            await sut.PushAsync(message1);
            await sut.PushAsync(message2);

            await sut.RemoveDataAsync(message1);

            var result = await sut.PopIndexAsync(message1.QueueName);

            Assert.NotNull(result);
            Assert.Equal(message2.Id, result.Id);

            result = await sut.PopIndexAsync(message1.QueueName);

            Assert.Null(result);
        }
    }
}
