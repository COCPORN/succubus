using Succubus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Succubus.Backend.Loopback;

namespace Succubus.Bus.Tests
{
    class InMemoryFactory : IFactory
    {
        public Core.Interfaces.IBus CreateBus()
        {
            IBus bus = new Succubus.Core.Bus();
            bus.Initialize(config => config.WithLoopback(clear: false));
            return bus;
        }

        public Core.Interfaces.IBus CreateBusWithHosting()
        {
            IBus bus = new Succubus.Core.Bus();
            bus.Initialize(config => config.WithLoopback(clear: true));
            return bus;
        }
    }
}
