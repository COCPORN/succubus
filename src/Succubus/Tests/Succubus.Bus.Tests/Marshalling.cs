using NUnit.Framework;
using Succubus.Backend.Loopback;
using Succubus.Bus.Tests.Messages;
using Succubus.Core.Interfaces;

namespace Succubus.Bus.Tests
{
    [TestFixture]
    public class Marshalling
    {
        private IBus bus;

        [SetUp]
        public void Init()
        {
            bus = Configuration.Factory.CreateBusWithHosting(config =>
                config.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse
                {
                    Message = req.Message
                }, marshal: (handler, request) =>
                {
                    return handler(request);
                })
            );
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