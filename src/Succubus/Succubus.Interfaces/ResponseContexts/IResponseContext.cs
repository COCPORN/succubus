using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Interfaces.ResponseContexts
{
    public interface IResponseContext
    {
        IResponseContext On<T>(Action<T> handler);

        IResponseContext Then<T>(Action<T> handler);        
    }
}
