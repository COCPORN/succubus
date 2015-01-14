using System.Threading;
using NUnit.Framework;
using Succubus.Backend.Loopback;
using Succubus.Bus.Tests.Messages;
using Succubus.Core.Interfaces;
using Succubus.Core.MessageFrames;

namespace Succubus.Bus.Tests
{

    [TestFixture]
    public class Raw
    {
        private IBus bus;

        [SetUp]
        public void Init()
        {
            bus = Configuration.Factory.CreateBusWithHosting(config => {
                config.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse
                {
                    Message = req.Message
                });

                config.OnRawMessage(o =>
                {
                    Assert.IsTrue(o is MessageBase);
                    Assert.IsTrue(o is Core.MessageFrames.Event);
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
            success = false;
            mre.Reset();

            bus.Publish(new BasicEvent() { Message = "Wohey" });
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