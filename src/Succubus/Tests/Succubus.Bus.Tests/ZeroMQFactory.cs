using Succubus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Succubus.Backend.ZeroMQ;
using Succubus.Hosting;
using System.Threading;

namespace Succubus.Bus.Tests
{
    class ZeroMQFactory : IFactory
    {
        public Core.Interfaces.IBus CreateBus()
        {
            IBus bus = new Succubus.Core.Bus();
            bus.Initialize(config => config.WithZeroMQ());
            return bus;
        }

        IBus hostingbus = null;
        public Core.Interfaces.IBus CreateBusWithHosting()
        {
            if (hostingbus == null)
            {
                hostingbus = new Succubus.Core.Bus();
                hostingbus.Initialize(config => config.WithZeroMQ(c => c.StartMessageHost()));
                Thread.Sleep(5000);
            }
            return hostingbus;
        }
    }
}
