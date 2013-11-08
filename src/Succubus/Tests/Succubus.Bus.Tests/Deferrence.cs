using System;
using System.CodeDom;
using System.Security.Cryptography;
using System.Threading;
using NUnit.Framework;
using Succubus.Bus.Tests.Messages;
using Succubus.Collections;
using Succubus.Collections.Interfaces;
using Succubus.Interfaces;

namespace Succubus.Bus.Tests
{
    [TestFixture]
    public class Deferrence
    {
        private Core.Bus bus;

        [SetUp]
        public void Init()
        {
            bus = new Core.Bus();

            bus.Initialize(succubus => succubus.ConfigureForTesting());

            bus.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse { Message = "FROM SERVER: " + req.Message });
            bus.ReplyTo<ChildRequest, ChildBase>(req => new ChildResponse1 { Message = "FROM SERVER: " + req.Message });

            bus.Defer<BasicRequest, BasicResponse>();
        }


        [Test]
        public void SimpleDeferrence()
        {


            var id = bus.Call(new BasicRequest { Message = "Testing deferrence" });

            bus.Pickup<BasicRequest, BasicResponse>(id, (req, res) =>
            {
                Assert.AreEqual("FROM SERVER: " + req.Message, res.Message);
            });
        }

        [Test]
        public void DoubleDeferrence()
        {
            Core.Bus bus2 = new Core.Bus();
            bus2.Initialize();

            Thread.Sleep(500);

            bus2.Defer<BasicRequest, BasicResponse>();

            var id = bus.Call(new BasicRequest() { Message = "Double deferrence" });

            bus.Pickup<BasicRequest, BasicResponse>(id, (req, res) =>
            {
                Assert.AreEqual("FROM SERVER: " + req.Message, res.Message);
            });

            bus2.Pickup<BasicRequest, BasicResponse>(id, (request, response) =>
            {
                Assert.AreEqual("FROM SERVER: " + request.Message, response.Message);
            });
        }

        [Test]
        public void REENABLE_BaseClassDeferrence()
        {

            return;
            var id = bus.Call(new ChildRequest() { Message = "wh00t" });

            bus.Pickup<ChildRequest, ChildBase>(id, (req, res) =>
            {
                Assert.AreEqual(typeof(ChildResponse1), res.GetType());
            });
        }
    }

}