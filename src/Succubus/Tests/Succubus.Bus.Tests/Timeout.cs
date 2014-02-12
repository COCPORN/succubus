using NUnit.Framework;
using Succubus.Backend.ZeroMQ;
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
                succubus.WithZeroMQ(config => config.StartMessageHost()));
        }

    }
}