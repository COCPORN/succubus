using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Succubus.Bus.Tests.Messages;

namespace Succubus.Bus.Tests
{
    [TestFixture]
    [Serializable]
    public class RequestResponse
    {
        private Core.Bus bus;

        [SetUp]
        public void Init()
        {
            bus = new Core.Bus();

            bus.Initialize(succubus => succubus.ConfigureForTesting());

            bus.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse
            {
                Message = req.Message
            });
        }


        [Test]
        public void SimpleReqResTransientRouteSynchronous()
        {
            var reply = bus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = "Hello" });
            Assert.AreEqual("Hello", reply.Message);
        }

        [Test]
        public async void SimpleReqResTransientRouteAsynchronous()
        {
            var reply = await bus.CallAsync<BasicRequest, BasicResponse>(new BasicRequest { Message = "Hello" });
            Assert.AreEqual("Hello", reply.Message);
        }
    }
}