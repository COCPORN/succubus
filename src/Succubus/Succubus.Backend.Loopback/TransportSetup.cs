using Succubus.Core.Interfaces;

namespace Succubus.Backend.Loopback
{
    public static class TransportSetup
    {
        public static void WithLoopback(this IBusConfigurator configurator, bool clear = false)
        {
            Transport transport = new Transport();
            transport.Bridge = configurator.Bridge;
            transport.Initialize(clear);
            configurator.Transport = transport;
            configurator.CorrelationIdProvider = transport;
            configurator.SubscriptionManager = transport;

        }
    }
}