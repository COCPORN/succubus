using Succubus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Bus.Tests
{
    interface IFactory
    {
        IBus CreateBus(Action<IBusConfigurator> configurator, bool reportRaw = false);
        IBus CreateBusWithHosting(Action<IBusConfigurator> configurator, bool reportRaw = false);        
    }
}
