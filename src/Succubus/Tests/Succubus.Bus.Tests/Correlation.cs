using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
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
        IBus overlapbus;
      
        [SetUp]
        public void Init()
        {

            overlapbus = Configuration.Factory.CreateBusWithHosting();



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
                tasks.Add(Task.Run(async () =>
                {
                    for (int j = 0; j < 20; j++)
                    {
                        string guid1 = Guid.NewGuid().ToString();
                        string guid2 = Guid.NewGuid().ToString();

                        var treply2 =
                            overlapbus.CallAsync<BasicRequest, BasicResponse>(new BasicRequest { Message = guid1 });
                        var treply =
                            overlapbus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = guid2 }, timeout:20000);
                        Assert.AreEqual(guid2, treply.Message);
                        Assert.AreEqual(guid1, (await treply2).Message);
                        lock (lockObject)
                        {
                            matchCount++;
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks);
            BusDiagnose.CheckDiagnose(overlapbus);

            Assert.AreEqual(200, matchCount);
            return;
        }


        [Test]
        public async void ThreadedRun()
        {
              object lockObject = new object();
            int matchCount = 0;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    for (int j = 0; j < 20; j++)
                    {
                        var treply2 =
                            overlapbus.CallAsync<BasicRequest, BasicResponse>(new BasicRequest { Message = "Hello" });
                        var treply =
                            overlapbus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = "Howdy" }, timeout:20000);
                        Assert.AreEqual("Howdy", treply.Message);
                        Assert.AreEqual("Hello", (await treply2).Message);
                        lock (lockObject)
                        {
                            matchCount++;
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks);
            BusDiagnose.CheckDiagnose(overlapbus);

            Assert.AreEqual(200, matchCount);
            return;
#if false

            object lockObject = new object();
            int matchCount = 0;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    for (int j = 0; j < 20; j++)
                    {
                        var treply2 =
                            overlapbus.CallAsync<BasicRequest, BasicResponse>(new BasicRequest { Message = "Hello" });
                        var treply =
                            overlapbus.Call<BasicRequest, BasicResponse>(new BasicRequest { Message = "Howdy" });
                        Assert.AreEqual("Howdy", treply.Message);
                        Assert.AreEqual("Hello", (await treply2).Message);
                        lock (lockObject)
                        {
                            matchCount++;
                        }
                    }
                }));
            }


            await Task.WhenAll(tasks);
            BusDiagnose.CheckDiagnose(overlapbus);

            Assert.AreEqual(200, matchCount);
            return;
#endif
        }

    }
}