using System.Threading;
using NUnit.Framework;
using Succubus.Backend.Loopback;
using Succubus.Backend.ZeroMQ;
using Succubus.Bus.Tests.Messages;
using Succubus.Hosting;

namespace Succubus.Bus.Tests
{
    [TestFixture]
    public class Timeout
    {
        private Core.Bus bus;

        [SetUp]
        public void Init()
        {
            bus = new Core.Bus();

            bus.Initialize(succubus =>
            {
                succubus.WithLoopback();
                succubus.CorrelationIdProvider = new PredeterminedCorrelationProvider();
            });
        }

        [Test]
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
            mre.WaitOne(500);

        }

    }
}