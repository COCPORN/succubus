﻿using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Succubus.Backend.Loopback;
using Succubus.Backend.ZeroMQ;
using Succubus.Bus.Tests.Messages;
using Succubus.Hosting;

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

            //bus.Initialize(succubus => succubus.WithZeroMQ(config => config.StartMessageHost()));
            //Thread.Sleep(1500);
            bus.Initialize(succubus => succubus.WithLoopback(clear: true));

            bus.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse
            {
                Message = req.Message
            });

            bus.ReplyTo<StaticRequest, StaticResponse>(req => new StaticResponse
            {
                Message = req.Message
            });

            bus.ReplyTo<ChildRequest, ChildBase>(req =>
            {
                if (req.Message == "Child1")
                {
                    return new ChildResponse1 { Message = req.Message };
                }
                else
                {
                    return new ChildResponse2 { Message = req.Message };
                }
            });
        }


        [Test]
        public async void SimpleReqResTransientRouteSynchronousAndAsynchronous()
        {
            for (int i = 0; i < 250; i++)
            {
                var reply2 = bus.CallAsync<BasicRequest, BasicResponse>(new BasicRequest { Message = "Hello" });
                var reply = bus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = "Howdy" });
                Assert.AreEqual("Howdy", reply.Message);
                Assert.AreEqual("Hello", (await reply2).Message);
            }
            BusDiagnose.CheckDiagnose(bus);
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
            BusDiagnose.CheckDiagnose(bus);
        }

        void OrchestrationSetup1()
        {

            bus.ReplyTo<Request1, Response2>(req => new Response2 { Message = "RESPONSE 2 " + req.Message });
            bus.ReplyTo<Request1, Response1>(req => new Response1 { Message = "RESPONSE 1 " + req.Message });
        }

        private void OrchestrationSetup2()
        {
            bus.ReplyTo<Request1, Response3>(req => new Response3 { Message = "RESPONSE 3 " + req.Message });
        }

        [Test]
        public void ComplexOrchestration1()
        {
            OrchestrationSetup1();

            bool res1res2in = false;
            bool res1res2res3in = false;
            var mre1 = new ManualResetEvent(false);
            var mre2 = new ManualResetEvent(false);

            bus.OnReply<Request1, Response1, Response2>((request, response1, response2) =>
            {
                res1res2in = true;
                mre1.Set();
            });

            bus.OnReply<Request1, Response1, Response2, Response3>((request, response1, response2, response3) =>
            {
                res1res2res3in = true;
                mre2.Set();
            });

            bus.Call(new Request1 { Message = "Hello!" }, timeout:500);
            

            //BusDiagnose.CheckDiagnose(bus);

            mre1.WaitOne(1500);
            mre2.WaitOne(1500);

            Assert.AreEqual(true, res1res2in);
            Assert.AreEqual(false, res1res2res3in);

            //Thread.Sleep(20000);

            BusDiagnose.CheckDiagnose(bus);
        }

        [Test]
        public void ComplexOrchestration2()
        {
            OrchestrationSetup1();
            OrchestrationSetup2();

            bool res1res2in = false;
            bool res1res2res3in = false;
            var mre1 = new ManualResetEvent(false);
            var mre2 = new ManualResetEvent(false);

            bus.OnReply<Request1, Response1, Response2>((request, response1, response2) =>
            {
                res1res2in = true;
                mre1.Set();
            });

            bus.OnReply<Request1, Response1, Response2, Response3>((request, response1, response2, response3) =>
            {
                res1res2res3in = true;
                mre2.Set();
            });

            bus.Call(new Request1 { Message = "Hello!" });

            mre2.WaitOne(500);

            Assert.AreEqual(true, res1res2in);
            Assert.AreEqual(true, res1res2res3in);
            BusDiagnose.CheckDiagnose(bus);
        }

        [Test]
        public void ChildMessages1_SimpleMapping()
        {
            var response = bus.Call<ChildRequest, ChildResponse1>(new ChildRequest { Message = "Child1" });
            Assert.AreEqual("Child1", response.Message);
            Assert.AreEqual(typeof(ChildResponse1), response.GetType());

            var response2 = bus.Call<ChildRequest, ChildResponse2>(new ChildRequest { Message = "Child2" });
            Assert.AreEqual("Child2", response2.Message);
            Assert.AreEqual(typeof(ChildResponse2), response2.GetType());
            BusDiagnose.CheckDiagnose(bus);
        }

        [Test]
        [ExpectedException(typeof(TimeoutException))]
        public void ChildMessages2_TimeoutOfMissingType()
        {
            bus.Call<ChildRequest, ChildResponse1>(new ChildRequest { Message = "Child2" }, timeout: 1000);            
            BusDiagnose.CheckDiagnose(bus);
        }

        [Test]
        public void ChildMessages3_HandlingOfBaseClassResponse()
        {
            var response = bus.Call<ChildRequest, ChildBase>(new ChildRequest { Message = "Child1" }, timeout: 10000);
            Assert.AreEqual("Child1", response.Message);
            Assert.AreEqual(typeof(ChildResponse1), response.GetType());

            var response2 = bus.Call<ChildRequest, ChildBase>(new ChildRequest { Message = "Child2" }, timeout: 10000);
            Assert.AreEqual("Child2", response2.Message);
            Assert.AreEqual(typeof(ChildResponse2), response2.GetType());
            BusDiagnose.CheckDiagnose(bus);
        }

        [Test]
        public void ChildMessages4_BaseClassOrchestrationTimeout()
        {           
            bus.OnReply<ChildRequest, ChildBase, ChildResponse2>((req, cb, cr2) =>
            {
                Assert.Fail("Got unexpected reply");
            });

            bus.Call(new ChildRequest { Message = "Child1" }, (req) =>
            {
                Console.WriteLine("Got timeout");
                //return;
            }, timeout: 1000);

            Thread.Sleep(2000);

            BusDiagnose.CheckDiagnose(bus);
        }

       
    }
}