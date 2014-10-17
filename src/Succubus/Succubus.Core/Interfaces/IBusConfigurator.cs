using System.Security.Cryptography.X509Certificates;

namespace Succubus.Core.Interfaces
{
    public interface IBusConfigurator
    {
        bool IncludeMessageOriginator { get; set; }
        ITransport Transport { get; set; }
        ISubscriptionManager SubscriptionManager { get; set; }
        ICorrelationIdProvider CorrelationIdProvider { get; set; }
        ITransportBridge Bridge { get; set; }
    }
}
