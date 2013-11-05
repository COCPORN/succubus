using System;
using System.Runtime.ExceptionServices;
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

            bus.ReplyTo<StaticRequest, StaticResponse>(req => new StaticResponse
            {
                Message = req.Message
            });
        }


        [Test]
        public async void SimpleReqResTransientRouteSynchronousAndAsynchronous()
        {
            for (int i = 0; i < 250; i++)
            {
                var reply2 = bus.CallAsync<BasicRequest, BasicResponse>(new BasicRequest {Message = "Hello"});
                var reply = bus.Call<BasicRequest, BasicResponse>(new BasicRequest {Message = "Howdy"});
                Assert.AreEqual("Howdy", reply.Message);
                Assert.AreEqual("Hello", (await reply2).Message);
            }
        }

     

        [Test]
        public void SimpleReqResStaticRouteSynchronous()
        {
            var mre = new ManualResetEvent(false);

            bus.OnReply<StaticRequest, StaticResponse>((req, res) =>
            {
                Assert.AreEqual("Hello", req.Message);
                Assert.AreEqual(req.Message, res.Message);
                mre.Set();
            });
            bus.Call(new StaticRequest { Message = "Hello" });
            if (mre.WaitOne(500) == false)
            {
                Assert.Fail("Timeout waiting for response");
            }

        }

    }
}