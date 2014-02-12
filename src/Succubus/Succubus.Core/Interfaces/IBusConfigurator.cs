using System.Security.Cryptography.X509Certificates;

namespace Succubus.Core.Interfaces
{
    public interface IBusConfigurator
    {
     
        ITransport Transport { get; set; }
        ISubscriptionManager SubscriptionManager { get; set; }

        ITransportBridge Bridge { get; set; }
    }
}
