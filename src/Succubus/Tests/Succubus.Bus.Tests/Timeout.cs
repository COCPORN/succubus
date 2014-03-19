using System;
using System.Threading;
using NUnit.Framework;
using Succubus.Backend.Loopback;
using Succubus.Backend.ZeroMQ;
using Succubus.Bus.Tests.Messages;
using Succubus.Core.Interfaces;
using Succubus.Hosting;

namespace Succubus.Bus.Tests
{
    [TestFixture]
    public class Timeout
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



            overlapbus.ReplyTo<BasicRequest, BasicResponse>(req =>
            {
                Thread.Sleep(500);
                return new BasicResponse
                {
                    Message = req.Message
                };
            });
        }


        [Test]
        public async void TimeoutOverlappingCorrelationIds()
        {
            for (int i = 0; i < 1; i++)
            {
                try
                {
                    var reply = overlapbus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = "Howdy" },
                        timeout: 100);
                    Assert.Fail("Call didn't timeout as expected");
                }
                catch (Exception ex)
                {
                    var reply = overlapbus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = "Howdy" },
                       timeout: 1000);
                    Assert.AreEqual("Howdy", reply.Message);
                }
              

            }
        }


    }
}