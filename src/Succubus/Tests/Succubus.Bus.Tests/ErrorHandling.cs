using System;
using System.Threading;
using NUnit.Framework;
using Succubus.Backend.Loopback;
using Succubus.Bus.Tests.Messages;

namespace Succubus.Bus.Tests
{
    [TestFixture]
    public class ErrorHandling
    {
        private Core.Bus bus;

        AutoResetEvent are = new AutoResetEvent(false);

        [SetUp]
        public void Init()
        {
            bus = new Core.Bus();
#if ZEROMQ_BACKEND
            bus.Initialize(succubus => succubus.WithZeroMQ(config => config.StartMessageHost()));
            Thread.Sleep(2500);
#else
            bus.Initialize(succubus => succubus.WithLoopback(clear: true));
#endif

            bus.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse
            {
                Message = req.Message
            });

            bus.ReplyTo<ErrorRequest, BasicResponse>(req =>
            {
                throw new Exception("Yeah, see, that doesn't really work");
            });

            bus.Exception += (sender, args) => are.Set();
        }

        [Test]
        [ExpectedException(typeof(TimeoutException))]
        public async void SimpleSynchronousError()
        {
            var reply = bus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = "Howdy" });
            Assert.AreEqual("Howdy", reply.Message);

            var errorReply = bus.CallAsync<ErrorRequest, BasicResponse>(new ErrorRequest { Message = "Wild snorlax" }, timeout:1000);
            if (are.WaitOne(1000) == false)
            {
                Assert.Fail("No exception raised");
            }

            await errorReply;
        }

    }
}