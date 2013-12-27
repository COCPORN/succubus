using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Succubus.Bus.Tests.Messages;
using Succubus.Hosting;

namespace Succubus.Bus.Tests
{
    [TestFixture]
    public class Events
    {
        private Core.Bus bus;

        [SetUp]
        public void Init()
        {
            bus = new Core.Bus();

            bus.Initialize(succubus => succubus.StartMessageHost());
            Thread.Sleep(1500);

            bus.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse
            {
                Message = req.Message
            });
        }

        [Test]
        public void SimpleEvent()
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
            });

            bus.Publish(new BasicEvent() { Message = "Wohey" });
            if (mre.WaitOne(500) == false)
            {
                Assert.Fail("Timeout waiting for event");
            }
            else
                Assert.AreEqual(1, counter);
        }


        [Test]
        public void InheritedEvent()
        {
            ManualResetEvent mre = new ManualResetEvent(false);

            bus.On<ParentEvent>(ev =>
            {
                mre.Set();
            });
            bus.Publish(new ChildEvent() { Message = "Child calling" });
            if (mre.WaitOne(500) == false)
            {
                Assert.Fail("Timeout waiting for event");

            }

        }

        [Test]
        public void ReqResAsEvents()
        {
            int counter = 0;
            ManualResetEvent mre = new ManualResetEvent(false);
         
            bus.On<object>(ev =>
            {
                Console.WriteLine("Got event: {0}", ev.ToString());
                if (ev is BasicRequest || ev is BasicResponse)
                {
                    counter++;

                }
                if (counter == 2)
                {
                    mre.Set();
                }
            });

            var response =
                bus.Call<BasicRequest, BasicResponse>(new BasicRequest()
                {
                    Message = "Testing eventing of synchronous messages"
                });
            if (mre.WaitOne(500) == false)
            {
                Assert.Fail("Timeout waiting for event");
            }
            else
            {
                Assert.AreEqual(2, counter);
                Assert.AreEqual(response.Message, "Testing eventing of synchronous messages");
            }
        }

        [Test]
        public void ReqResAsEventsSecondBusInstance()
        {
            var bus2 = new Core.Bus();
            bus2.Initialize();
            Thread.Sleep(1500);

            int counter = 0;
            ManualResetEvent mre = new ManualResetEvent(false);
      
            bus2.On<object>(ev =>
            {
                Console.WriteLine("Got event: {0}", ev.ToString());
                if (ev is BasicRequest || ev is BasicResponse)
                {
                    counter++;

                }
                if (counter == 2)
                {
                    mre.Set();
                }
            });

            var response =
                bus.Call<BasicRequest, BasicResponse>(new BasicRequest()
                {
                    Message = "Testing eventing of synchronous messages"
                });
            if (mre.WaitOne(500) == false)
            {
                Assert.Fail("Timeout waiting for event");
            }
            else
            {
                Assert.AreEqual(2, counter);
                Assert.AreEqual(response.Message, "Testing eventing of synchronous messages");
            }
        }
    }

}
