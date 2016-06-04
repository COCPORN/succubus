using System;
using Succubus.Core.Interfaces;
using Succubus.Serialization;

namespace Succubus.Backend.NetMQ
{
    public static class TransportSetup
    {
        public static IPostConfigurator WithNetMQ(this IBusConfigurator configurator)
        {
            Transport transport = new Transport();
            transport.Configurator = configurator;
            transport.Bridge = configurator.Bridge;
       
  
            configurator.Transport = transport;
            configurator.SubscriptionManager = transport;
            configurator.CorrelationIdProvider = transport;
            transport.Initialize();
            if (transport.SubscriberOnline.WaitOne(3000) == false)
            {
                throw new Exception("Subscriber thread timeout");
            }
            return transport;
        }

        public static IPostConfigurator WithNetMQ(this IBusConfigurator configurator,
            Action<INetMQConfigurator> initializationHandler)
        {
            Transport transport = new Transport();
            initializationHandler(transport);
            transport.Configurator = configurator;
            transport.Bridge = configurator.Bridge;
       
            configurator.Transport = transport;
            configurator.SubscriptionManager = transport;
            configurator.CorrelationIdProvider = transport;
            transport.Initialize();
            if (transport.SubscriberOnline.WaitOne(3000) == false)
            {
                throw new Exception("Subscriber thread timeout");
            }
            return transport;
        }
    }
}