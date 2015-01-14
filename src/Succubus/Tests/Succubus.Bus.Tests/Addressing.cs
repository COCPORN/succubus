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
using Succubus.Core.Interfaces;

namespace Succubus.Bus.Tests
{
    [TestFixture]
    class Addressing
    {

        private IBus bus;

        [SetUp]
        public void Init()
        {
            bus = Configuration.Factory.CreateBusWithHosting(config =>
                {
                    config.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse
                    {
                        Message = req.Message
                    }, "ADDRESS");

                    config.On<BasicEvent>(ev =>
                    {
                        if (ev.Message == "Wohey")
                        {
                            counter++;
                            mre.Set();
                        }
                    }, "NOTADDRESS");
                    config.On<BasicEvent>(ev =>
                    {
                        if (ev.Message == "Wohey")
                        {
                            counter++;
                            mre.Set();
                        }
                    }, "CORRECTADDRESS");
                });
            
         
        }

        int counter = 0;
        ManualResetEvent mre = new ManualResetEvent(false);

        [Test]
        public void SimpleEventWithIncorrectAddressing()
        {
            counter = 0;
            mre.Reset();
         
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

        [Test]
        public void SimpleEventWithCorrectAddressing()
        {
            int counter = 0;
            ManualResetEvent mre = new ManualResetEvent(false);
                    
            bus.Publish(new BasicEvent() { Message = "Wohey" }, "CORRECTADDRESS");
            if (mre.WaitOne(2500) == false)
            {
                Assert.Fail("Timeout waiting for event");
            }
            else
                Assert.AreEqual(1, counter);
        }

    }
}
