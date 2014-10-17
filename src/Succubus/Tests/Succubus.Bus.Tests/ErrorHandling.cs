using System;
using System.Threading;
using NUnit.Framework;
using Succubus.Backend.Loopback;
using Succubus.Bus.Tests.Messages;
using Succubus.Core.Interfaces;

namespace Succubus.Bus.Tests
{
    [TestFixture]
    public class ErrorHandling
    {
        private IBus bus;
  
        readonly AutoResetEvent are = new AutoResetEvent(false);

        [SetUp]
        public void Init()
        {
            bus = Configuration.Factory.CreateBusWithHosting();
            

            bus.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse
            {
                Message = req.Message
            });

            bus.ReplyTo<ErrorRequest, BasicResponse>(req =>
            {
                throw new Exception("Yeah, see, that doesn't really work");
            });

            bus.HandlerException += (sender, args) => are.Set();
        }

        [Test]
        [ExpectedException(typeof(TimeoutException))]
        public async void SimpleSynchronousError()
        {
            var reply = bus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = "Howdy" });
            Assert.AreEqual("Howdy", reply.Message);

            var errorReply = bus.CallAsync<ErrorRequest, BasicResponse>(new ErrorRequest { Message = "Wild snorlax" }, timeout:100);
            if (are.WaitOne(1000) == false)
            {
                Assert.Fail("No exception raised");
            }

            await errorReply;
        }

    }
}