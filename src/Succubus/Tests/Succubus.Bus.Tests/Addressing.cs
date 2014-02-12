using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Succubus.Backend.Loopback;
using Succubus.Backend.ZeroMQ;
using Succubus.Bus.Tests.Messages;
using Succubus.Hosting;

namespace Succubus.Bus.Tests
{
    [TestFixture]
    class Addressing
    {

        private Core.Bus bus;

        [SetUp]
        public void Init()
        {
            bus = new Core.Bus();
            //bus.Initialize(succubus => succubus.WithZeroMQ(config => config.StartMessageHost()));
            bus.Initialize(succubus => succubus.WithLoopback());

            bus.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse
            {
                Message = req.Message
            }, "ADDRESS");
        }

        [Test]
        public void SimpleEventWithCorrectAddressing()
        {
            int counter = 0;
            ManualResetEvent mre = new ManualResetEvent(false);
            bus.On<BasicEvent>(ev =>
            {
                if (ev.Message == "Wohey")
                {
                    counter++;
                    mre.Set();
                }
            }, "CORRECTADDRESS");
            bus.Publish(new BasicEvent() { Message = "Wohey" }, "CORRECTADDRESS");
            if (mre.WaitOne(500) == false)
            {
                Assert.Fail("Timeout waiting for event");
            }
            else
                Assert.AreEqual(1, counter);
        }

        [Test]
        public void SimpleEventWithIncorrectAddressing()
        {
            int counter = 0;
            ManualResetEvent mre = new ManualResetEvent(false);
            bus.On<BasicEvent>(ev =>
            {
                if (ev.Message == "Wohey")
                {
                    counter++;
                    mre.Set();
                }
            }, "NOTADDRESS");

            bus.Publish(new BasicEvent() { Message = "Wohey" }, "ADDRESS");
            if (mre.WaitOne(500) == false)
            {
                return;
            }
            else
                Assert.Fail("This call should time out");
        }

        [Test]
        public async void SimpleSynchronousWithCorrectAddressing()
        {
            for (int i = 0; i < 250; i++)
            {
                var reply2 = bus.CallAsync<BasicRequest, BasicResponse>(new BasicRequest { Message = "Hello" }, "ADDRESS");
                var reply = bus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = "Howdy" }, "ADDRESS");
                Assert.AreEqual("Howdy", reply.Message);
                Assert.AreEqual("Hello", (await reply2).Message);
            }
        }

        [Test]
        [ExpectedException(typeof(TimeoutException))]
        public void SimpleSynchronousWithIncorrectAddressing()
        {
            var reply = bus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = "Howdy" }, "BADADDRESS", 1000);

        }



    }
}
