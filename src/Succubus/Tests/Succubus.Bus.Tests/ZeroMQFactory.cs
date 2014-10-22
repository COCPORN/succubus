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
        public Core.Interfaces.IBus CreateBus(bool reportRaw = false)
        {
            IBus bus = new Succubus.Core.Bus();
            bus.Initialize(config => {
                config.WithZeroMQ();
                var transport = config.Transport as Succubus.Backend.ZeroMQ.Transport;
                if (reportRaw == true)
                {
                    transport.ReportRaw = true;
                }
            });

            return bus;
        }

        IBus hostingbus = null;
        public Core.Interfaces.IBus CreateBusWithHosting(bool reportRaw = false)
        {
            if (hostingbus == null)
            {
                hostingbus = new Succubus.Core.Bus();
                hostingbus.Initialize(config => {
                    config.WithZeroMQ(c => c.StartMessageHost());
                    var transport = config.Transport as Succubus.Backend.ZeroMQ.Transport;
                    if (reportRaw == true)
                    {
                        transport.ReportRaw = true;
                    }
                    
                });
                Thread.Sleep(1000);
            }
            return hostingbus;
        }
    }
}
