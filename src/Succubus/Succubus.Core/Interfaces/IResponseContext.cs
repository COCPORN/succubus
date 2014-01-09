using System;

namespace Succubus.Core.Interfaces
{
    public interface IResponseContext
    {
        IResponseContext On<T>(Action<T> handler);

        IResponseContext Then<T>(Action<T> handler);        
    }
}
