using System.IO;

namespace Succubus.Core.Interfaces
{
    public interface IBusConfigurator
    {
        bool IncludeMessageOriginator { get; set; }
        TextWriter LogWriter { get; set; }
        LogLevel LogLevel { get; set; }
        string Name { get; set; }
        ITransport Transport { get; set; }
        ISubscriptionManager SubscriptionManager { get; set; }
        ICorrelationIdProvider CorrelationIdProvider { get; set; }
        ITransportBridge Bridge { get; set; }

        #region Handlers

        #endregion
    }
}
