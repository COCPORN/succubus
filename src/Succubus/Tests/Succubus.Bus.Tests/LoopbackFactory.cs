﻿using Succubus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Succubus.Backend.Loopback;

namespace Succubus.Bus.Tests
{
    class LoopbackFactory : IFactory
    {
        public Core.Interfaces.IBus CreateBus(bool reportRaw = false)
        {
            IBus bus = new Succubus.Core.Bus();
            bus.Initialize(config => {
                config.WithLoopback(clear: false);
                if (reportRaw == true)
                {
                    (config.Transport as Succubus.Backend.Loopback.Transport).ReportRaw = true;
                }                
            });
            return bus;
        }

        public Core.Interfaces.IBus CreateBusWithHosting(bool reportRaw = false)
        {
            IBus bus = new Succubus.Core.Bus();
            bus.Initialize(config =>
            {                
                config.WithLoopback(clear: true);
                if (reportRaw == true) { 
                    (config.Transport as Succubus.Backend.Loopback.Transport).ReportRaw = true;                
                }
            });
            return bus;
        }
    }
}
