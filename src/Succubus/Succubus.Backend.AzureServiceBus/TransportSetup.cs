using Succubus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Backend.AzureServiceBus
{
    public static class TransportSetup
    {
        public static void WithAzureServiceBus(this IBusConfigurator configurator)
        {
        }

        public static void WithAzureServiceBus(this IBusConfigurator configurator, Action<IAzureServiceBusConfigurator> initializationHandler)
        {
            var transport = new Transport();
            initializationHandler(transport);            
        }
    }
}
