using NUnit.Framework;
using Succubus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Succubus.Backend.Loopback;
using System.IO;

namespace Succubus.Bus.Tests
{
    [TestFixture]
    class Diagnostics
    {

        private IBus bus;
        IBus bus2;

        [SetUp]
        public void Init()
        {
            bus = Configuration.Factory.CreateBusWithHosting(config => { });
            bus2 = Configuration.Factory.CreateBus(config => { });
        }

        [Test]
        public void CheckNames()
        {
            Assert.AreNotEqual(bus.Name, bus2.Name);
            Console.WriteLine("Bus name: {0}", bus.Name);
            Console.WriteLine("Bus2 name: {0}", bus2.Name);
        }

        [Test]
        public void CheckLogging()
        {
            var bus = new Succubus.Core.Bus();
            var memoryStream = new MemoryStream();
            bus.Initialize(config =>
            {
                config.WithLoopback();
                config.Name = "TestBus";                
                config.LogWriter = new StreamWriter(memoryStream);
                config.LogLevel = Core.LogLevel.Trace;
            });

            memoryStream.Position = 0;
            var sr = new StreamReader(memoryStream);
            var myStr = sr.ReadToEnd();
           
            Assert.AreEqual("[Info] TestBus: Bus initialized\r\n", myStr);
        }
    }
}
