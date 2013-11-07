using System;
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
        }

        [Test]
        public void ManualDeferrence()
        {
            var deferredContexts = new TimeoutHandler<Guid, DeferredContext<BasicResponse>>();

            // Callsite: Defer the request
            bus.OnReply<BasicRequest, BasicResponse>((req, res) =>
            {
                deferredContexts.Timeout(new DeferredContext<BasicResponse>{ Response = res }, 60000);
            });


            // Pickupsite
            var id = bus.Call(new BasicRequest { Message = "WOHEY!" });
        }


        [Test]
        public void SimpleDeferrence()
        {
            bus.Defer<BasicRequest, BasicResponse>();

            var id = bus.Call(new BasicRequest { Message = "Testing deferrence" });

            Thread.Sleep(1000);

            bus.Pickup<BasicRequest, BasicResponse>(id, (req, res) =>
            {
                Assert.AreEqual(req.Message, res.Message);
            });
        }
    }

    public class DeferredContext<T> : IExpiring<Guid>
    {
        public T Response { get; set; }
        public bool TimedOut { get; set; }
        public Action TimeoutHandler { get; private set; }
        public Guid Id { get; private set; }
    }
}