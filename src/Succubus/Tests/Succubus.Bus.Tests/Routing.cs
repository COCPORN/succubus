using System.Threading;
using NUnit.Framework;
using Succubus.Backend.Loopback;
using Succubus.Backend.ZeroMQ;
using Succubus.Bus.Tests.Messages;
using Succubus.Hosting;
using Succubus.Core.Interfaces;

namespace Succubus.Bus.Tests
{ 
    [TestFixture]
    public class Routing
    {
        private IBus bus;

        [SetUp]
        public void Init()
        {
            bus = Configuration.Factory.CreateBusWithHosting(config =>
            {
                // Setup chaining
                config.ReplyTo<C1, C2>(c => new C2());
                config.ReplyTo<C2, C3>(c => new C3());
                config.ReplyTo<C3, C4>(c => new C4());

                // Complex routing
                config.ReplyTo<A, B1>(a =>
                    new B1()
                    );
                config.ReplyTo<A, B2>(a => new B2());
                config.ReplyTo<B1, C>(b1 => new D1());
                config.ReplyTo<B2, C>(b2 => new D2());
                config.ReplyTo<C, Rb>(c => c is D1 ? (Rb)new R1 { C = c } : (Rb)new R2 { C = c });
                //Thread.Sleep(1000);

                config.OnReply<C1, C4>((req, res) =>
                {
                    mre.Set();
                });

                config.OnReply<A, Rb>((req, res) =>
                {
                    Assert.AreEqual(typeof(A), req.GetType());
                    Assert.True(res.C is D1 && res.GetType() == typeof(R1) ||
                        res.C is D2 && res.GetType() == typeof(R2));
                    mre.Set();
                });
            });                        
        }

        ManualResetEvent mre = new ManualResetEvent(false);

        [Test]
        public void SimpleChaining()
        {

            mre.Reset();
         
            bus.Call(new C1());
            if (mre.WaitOne(2500) == false)
            {
                Assert.Fail("Call timed out");
            }

        }

        [Test]
        // This currently doesn't work because ReplyTo doesn't work
        // with child classes
        public void TestAdvancedRouting()
        {

            mre.Reset();


            bus.Call(new A());
            if (mre.WaitOne(2500) == false)
            {
                Assert.Fail("Call timed out");
            }

        }
    }
}