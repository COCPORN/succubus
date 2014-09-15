using System.Threading;
using NUnit.Framework;
using Succubus.Backend.Loopback;
using Succubus.Backend.ZeroMQ;
using Succubus.Bus.Tests.Messages;
using Succubus.Hosting;

namespace Succubus.Bus.Tests
{ 
    [TestFixture]
    public class Routing
    {
        private Core.Bus bus;

        [SetUp]
        public void Init()
        {
            bus = new Core.Bus();

            bus.Initialize(succubus => succubus.WithLoopback(clear: true));

            // Setup chaining
            bus.ReplyTo<C1, C2>(c => new C2());
            bus.ReplyTo<C2, C3>(c => new C3());
            bus.ReplyTo<C3, C4>(c => new C4());

            // Complex routing
            bus.ReplyTo<A, B1>(a =>
                new B1()
                );
            bus.ReplyTo<A, B2>(a => new B2());
            bus.ReplyTo<B1, C>(b1 => new D1());
            bus.ReplyTo<B2, C>(b2 => new D2());
            bus.ReplyTo<C, Rb>(c => c is D1 ? (Rb)new R1 { C = c } : (Rb)new R2 { C = c });
            //Thread.Sleep(1000);
        }

        [Test]
        public void SimpleChaining()
        {
            var mre = new ManualResetEvent(false);

            bus.OnReply<C1, C4>((req, res) =>
            {               
                mre.Set();
            });
            bus.Call(new C1());
            if (mre.WaitOne(2500) == false)
            {
                Assert.Fail("Call timed out");
            }

        }

        [Test]
        // Unfortunately, routing doesn't seem to work as expected, as ReplyTo-handlers
        // will not fire from other replyto-handlers
        public void TestAdvancedRouting()
        {
        
            var mre = new ManualResetEvent(false);

            bus.OnReply<A, Rb>((req, res) =>
            {
                Assert.AreEqual(typeof(A), req.GetType());
                Assert.True(res.C is D1 && res.GetType() == typeof(R1) ||
                    res.C is D2 && res.GetType() == typeof(R2));
                mre.Set();
            });
            bus.Call(new A());
            if (mre.WaitOne(2500) == false)
            {
                Assert.Fail("Call timed out");
            }

        }
    }
}