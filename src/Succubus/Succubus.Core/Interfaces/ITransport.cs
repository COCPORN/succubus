using System;

namespace Succubus.Core.Interfaces
{
    public interface ITransport
    {
        void ObjectPublish(object message, string address, Action<Action> marshal = null);        
    }
}