using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Succubus.Bus.Tests.Messages;
using Succubus.Core.Interfaces;
using Succubus.Backend.Loopback;

namespace Succubus.Bus.Tests
{
    [TestFixture]
    public class Correlation
    {

        IBus overlapbus = new Core.Bus();

        [SetUp]
        public void Init()
        {

            overlapbus.Initialize(succubus =>
            {
                succubus.WithLoopback();
                succubus.CorrelationIdProvider = new PredeterminedCorrelationProvider();
            });



            overlapbus.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse
            {
                Message = req.Message
            });
        }


        [Test]
        public async void CorrelationOverlap()
        {
            for (int i = 0; i < 10; i++)
            {
                var reply2 = overlapbus.CallAsync<BasicRequest, BasicResponse>(new BasicRequest { Message = "Hello" });
                var reply = overlapbus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = "Howdy" });
                Assert.AreEqual("Howdy", reply.Message);
                Assert.AreEqual("Hello", (await reply2).Message);
            }
        }

    }

    class GuidCorrelationProvider : ICorrelationIdProvider
    {
        public string CreateCorrelationId(object o)
        {
            return Guid.NewGuid().ToString();
        }
    }
}