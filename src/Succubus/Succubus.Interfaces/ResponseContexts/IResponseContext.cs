using System;

namespace Succubus.Interfaces.ResponseContexts
{
    public interface IResponseContext
    {
        IResponseContext On<T>(Action<T> handler);

        IResponseContext Then<T>(Action<T> handler);        
    }
}
