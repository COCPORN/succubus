using System.Threading;
using NUnit.Framework;
using Succubus.Backend.Loopback;
using Succubus.Bus.Tests.Messages;
using Succubus.Core.Interfaces;

namespace Succubus.Bus.Tests
{

    [TestFixture]
    public class Frames
    {
        private Core.Bus bus;

        [SetUp]
        public void Init()
        {
            bus = new Core.Bus();
#if ZEROMQ_BACKEND
            bus.Initialize(succubus => succubus.WithZeroMQ(config => config.StartMessageHost()));
            Thread.Sleep(2500);
#else
            bus.Initialize(succubus => succubus.WithLoopback(clear: true, loopbackConfigurator: config => config.ReportRaw = true));
#endif

            bus.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse
            {
                Message = req.Message
            });
        }


        [Test]
        public void Test()
        {
            bool success = false;
            ManualResetEvent mre = new ManualResetEvent(false);
            bus.On<IMessageFrame>(o =>
            {
                Assert.IsTrue(o is Core.MessageFrames.Event);
                Assert.IsTrue((o as Core.MessageFrames.Event).Message is BasicEvent);
                success = true;
                mre.Set();
            });

            bus.Publish(new BasicEvent() { Message = "Wohey"});
            if (mre.WaitOne(1000) == false)
            {
                Assert.Fail("Failed getting raw message");
            }
            else
            {
                Assert.IsTrue(success);
            }

        }
    }


}