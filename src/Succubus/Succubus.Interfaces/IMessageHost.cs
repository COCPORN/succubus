using System;

namespace Succubus.Interfaces
{
    public interface IMessageHost
    {
        #region Initialization
        void Initialize();
        void Initialize(Action<IHostConfigurator> initializationHandler);
        #endregion

        event EventHandler<ProcessedMessageEventArgs> ProcessedMessage;

        void Start();
        void Stop();
    }
}
