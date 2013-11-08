using System;
using System.Security.Cryptography;
using System.Threading;
using NUnit.Framework;
using Succubus.Bus.Tests.Messages;
using Succubus.Collections;
using Succubus.Collections.Interfaces;

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

            bus.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse { Message = req.Message });
            bus.ReplyTo<ChildRequest, ChildBase>(req => new ChildResponse1 { Message = req.Message });
        }


        [Test]
        public void SimpleDeferrence()
        {
            bus.Defer<BasicRequest, BasicResponse>();

            var id = bus.Call(new BasicRequest { Message = "Testing deferrence" });

            bus.Pickup<BasicRequest, BasicResponse>(id, (req, res) =>
            {
                Assert.AreEqual(req.Message, res.Message);
            });
        }

        [Test]
        public void BaseClassDeferrence()
        {
            bus.Defer<ChildRequest, ChildBase>();

            var id = bus.Call(new ChildRequest() { Message = "wh00t" });

            bus.Pickup<ChildRequest, ChildBase>(id, (req, res) =>
            {
                Assert.AreEqual(typeof(ChildResponse1), res.GetType());
            });
        }
    }

}