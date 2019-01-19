using Aguacongas.RedisQueue.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.RedisQueue.Web.Test
{
    public class QueuesControllerTest
    {
        [Fact]
        public  async Task Post_should_sanatize_queue_name()
        {
            Mock<IManageQueues> queueManagerMock;
            QueuesController sut;
            CreateSut(out queueManagerMock, out sut);

            await sut.Post("http:/test", "test");

            queueManagerMock.Verify(m => m.EnqueueAsync(It.Is<Message>(msg => msg.QueueName == "http://test")));
        }

        [Fact]
        public async Task Put_should_sanatize_queue_name()
        {
            Mock<IManageQueues> queueManagerMock;
            QueuesController sut;
            CreateSut(out queueManagerMock, out sut);

            await sut.Put("test", new Model.Message
            {
            });

            queueManagerMock.Verify(m => m.EnqueueAsync(It.Is<Message>(msg => msg.QueueName == "test")));

            await sut.Put("https:/test", new Model.Message
            {
            });

            queueManagerMock.Verify(m => m.EnqueueAsync(It.Is<Message>(msg => msg.QueueName == "https://test")));
        }

        private static void CreateSut(out Mock<IManageQueues> queueManagerMock, out QueuesController sut)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "test test";
            var controllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            queueManagerMock = new Mock<IManageQueues>();
            queueManagerMock.Setup(m => m.EnqueueAsync(It.IsAny<Message>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            sut = new QueuesController(queueManagerMock.Object)
            {
                ControllerContext = controllerContext
            };
        }
    }
}
