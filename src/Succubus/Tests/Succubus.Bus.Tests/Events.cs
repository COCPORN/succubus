using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Succubus.Backend.Loopback;
using Succubus.Backend.ZeroMQ;
using Succubus.Bus.Tests.Messages;
using Succubus.Core.MessageFrames;
using Succubus.Hosting;
using Succubus.Core.Interfaces;

namespace Succubus.Bus.Tests
{
    [TestFixture]
    public class Events
    {
        private IBus bus;

        [SetUp]
        public void Init()
        {

            bus = new Succubus.Core.Bus();
            bus.Initialize(succubus =>
            {
                succubus.WithLoopback(config => config.ReportRaw = true, clear: true);
                succubus.IncludeMessageOriginator = true;
            });


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

            BusDiagnose.CheckDiagnose(bus);
        }


        [Test]
        public void CheckOriginator()
        {
            int counter = 0;
            string machineName = String.Empty;
            ManualResetEvent mre = new ManualResetEvent(false);
            bus.OnRaw((o) =>
            {
                var b = o as MessageBase;
                if (b != null)
                {
                    Console.WriteLine("Got the request: " + o + " MachineName: " + b.Originator);
                    counter++;
                    machineName = b.Originator;
                    mre.Set();
                }
            });

            bus.Publish(new BasicEvent() { Message = "Wohey" });
            if (mre.WaitOne(500) == false)
            {
                Assert.Fail("Timeout waiting for event");
            }
            else
            {
                Assert.AreEqual(1, counter);
                Assert.AreNotEqual(String.Empty, machineName);
            }

            BusDiagnose.CheckDiagnose(bus);
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
            if (mre.WaitOne(2500) == false)
            {
                Assert.Fail("Timeout waiting for event");

            }
            BusDiagnose.CheckDiagnose(bus);
        }

        [Test]
        public void InterfaceDispatch()
        {
            ManualResetEvent mre = new ManualResetEvent(false);

            bus.On<Marker>(ev =>
            {
                mre.Set();
            });
            bus.Publish(new ChildEvent() { Message = "Child calling" });
            if (mre.WaitOne(500) == false)
            {
                Assert.Fail("Timeout waiting for event");
            }
            BusDiagnose.CheckDiagnose(bus);
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
            BusDiagnose.CheckDiagnose(bus);
        }

        [Test]
        public void ReqResAsEventsSecondBusInstance()
        {
            var bus2 = new Core.Bus();

#if ZEROMQ_BACKEND
    
            bus2.Initialize(config => config.WithZeroMQ());
            Thread.Sleep(1500);
#else
            bus2.Initialize(config => config.WithLoopback());
#endif
            int counter = 0;
            ManualResetEvent mre = new ManualResetEvent(false);

            bus2.On<object>(ev =>
            {
                Console.WriteLine("Got event: {0}", ev.ToString());
                if (ev is BasicRequest || ev is BasicResponse || ev is BasicEvent)
                {
                    counter++;

                }
                if (counter == 3)
                {
                    mre.Set();
                }
            });

            var response =
                bus.Call<BasicRequest, BasicResponse>(new BasicRequest()
                {
                    Message = "Testing eventing of synchronous messages"
                });
            bus.Publish(new BasicEvent() { Message = "Testing catchall of events" });
            if (mre.WaitOne(500) == false)
            {
                Assert.Fail("Timeout waiting for event");
            }
            else
            {
                Assert.AreEqual(3, counter);
                Assert.AreEqual(response.Message, "Testing eventing of synchronous messages");
            }
            BusDiagnose.CheckDiagnose(bus);
        }
    }

}
