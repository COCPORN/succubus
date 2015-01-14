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
        private IBus bus;

        [SetUp]
        public void Init()
        {
            bus = Configuration.Factory.CreateBusWithHosting(config =>
            {
                config.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse
                {
                    Message = req.Message
                });

                config.On<IMessageFrame>(o =>
                {
                    Assert.IsTrue(o is Core.MessageFrames.Event);
                    Assert.IsTrue((o as Core.MessageFrames.Event).Message is BasicEvent);
                    success = true;
                    mre.Set();
                });                
            }, true);           
        }

        bool success = false;

        ManualResetEvent mre = new ManualResetEvent(false);

        [Test]
        public void Test()
        {
            mre.Reset();
            success = false;
          
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