using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Succubus.Core.Interfaces;

namespace Succubus.Backend.Caliburn.Micro.EventAggregator
{
    public static class TransportSetup
    {
        public static void WithEventAggregator(IBusConfigurator configurator, IEventAggregator aggregator)
        {
            Transport transport = new Transport();
            transport.EventAggregator = aggregator;

        }
    }
}
