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
        IBus bus2;

        [SetUp]
        public void Init()
        {

            bus = Configuration.Factory.CreateBusWithHosting(config => {
                config.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse
                {
                    Message = req.Message
                });
                config.On<BasicEvent>(ev =>
                {
                    if (ev.Message == "Wohey")
                    {
                        counter++;
                        mre.Set();
                    }
                });
                config.On<ParentEvent>(ev =>
                {
                    mre.Set();
                });
                config.On<Marker>(ev =>
                {
                    mre.Set();
                });
                config.On<object>(ev =>
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
                config.On<object>(ev =>
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
            }, true);
            bus2 = Configuration.Factory.CreateBus(config => {
                config.OnRawMessage((o) =>
                {
                    Console.WriteLine(o.ToString());
                    var b = o as MessageBase;
                    if (b != null)
                    {
                        Console.WriteLine("Got the request: " + o + " MachineName: " + b.Originator);
                        counter++;
                        machineName = b.Originator;
                        mre.Set();
                    }
                });
            }, true);
         
        
        }

        int counter = 0;
        ManualResetEvent mre = new ManualResetEvent(false);
         

        [Test]
        public void SimpleEvent()
        {
            mre.Reset();
            counter = 0;
            bus.Publish(new BasicEvent() { Message = "Wohey" });
            if (mre.WaitOne(500) == false)
            {
                Assert.Fail("Timeout waiting for event");
            }
            else
                Assert.AreEqual(1, counter);

            BusDiagnose.CheckDiagnose(bus);
        }

        string machineName = String.Empty;

        [Test]
        public void CheckOriginator()
        {
            counter = 0;
            machineName = String.Empty;
            mre.Reset();
          

            bus.Publish(new BasicEvent() { Message = "Wohey" });
            if (mre.WaitOne(1500) == false)
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
            mre.Reset();
            
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
            mre.Reset();
        
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
            var bus2 = Configuration.Factory.CreateBus(config => { });
            //Thread.Sleep(1000);

            int counter = 0;
            ManualResetEvent mre = new ManualResetEvent(false);

           

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
