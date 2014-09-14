using System;

namespace Succubus.Core.Interfaces
{
    public interface ITransport
    {
        void BusPublish(object message, string address, Action<Action> marshal = null);
    }
}