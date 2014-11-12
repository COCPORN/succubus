using System;

namespace Succubus.Core.Interfaces
{
    public interface IBusTransport
    {
        void BusPublish(object message, string address, Action<Action> marshal = null);     
    }
}