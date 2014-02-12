using System;
using Succubus.Core.Interfaces;
using Succubus.Serialization;

namespace Succubus.Backend.ZeroMQ
{
    public static class TransportSetup
    {
        public static void WithZeroMQ(this IBusConfigurator configurator)
        {
            Transport transport = new Transport();
            transport.Configurator = configurator;
            transport.Bridge = configurator.Bridge;
       
  
            configurator.Transport = transport;
            configurator.SubscriptionManager = transport;

            transport.Initialize();
        }

        public static void WithZeroMQ(this IBusConfigurator configurator,
            Action<IZeroMQConfigurator> initializationHandler)
        {
            Transport transport = new Transport();
            initializationHandler(transport);
            transport.Configurator = configurator;
            transport.Bridge = configurator.Bridge;
       
            configurator.Transport = transport;
            configurator.SubscriptionManager = transport;
            transport.Initialize();
        }
    }
}