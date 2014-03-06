using NUnit.Framework;
using Succubus.Backend.Loopback;
using Succubus.Bus.Tests.Messages;

namespace Succubus.Bus.Tests
{
    [TestFixture]
    public class Marshalling
    {
        private Core.Bus bus;

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
            }, marshal: (handler, request) =>
            {
                return handler(request);
            });
        }

        [Test]
        public void Unmarshalled()
        {
            var reply = bus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = "Howdy" });
            Assert.AreEqual("Howdy", reply.Message);
        }

        [Test]
        public void Marshalled_1()
        {
            var reply = bus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = "Howdy" });
            Assert.AreEqual("Howdy", reply.Message);
        }
    }
}