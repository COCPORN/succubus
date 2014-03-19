using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
                //succubus.CorrelationIdProvider = new PredeterminedCorrelationProvider();
            });



            overlapbus.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse
            {
                Message = req.Message
            });
        }


        [Test]
        public async void CorrelationOverlap()
        {
            object lockObject = new object();
            
            int matchCount = 0;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        //Console.WriteLine("{0}:{1}", i, j);
                        try
                        {
                            var reply2 =
                                overlapbus.CallAsync<BasicRequest, BasicResponse>(new BasicRequest { Message = "Hello" });
                            var reply =
                                overlapbus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = "Howdy" });
                            Assert.AreEqual("Howdy", reply.Message);
                            Assert.AreEqual("Hello", (await reply2).Message);
                            lock (lockObject)
                            {
                                matchCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception in call: {0}", ex.ToString());
                        }
                    }
                }));

            }
            Console.WriteLine(tasks.Count);
            await Task.WhenAll(tasks);
            Assert.AreEqual(1000, matchCount);
        }

    }
}