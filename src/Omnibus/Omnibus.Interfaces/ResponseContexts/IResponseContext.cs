using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnibus.Interfaces.ResponseContexts
{
    public interface IResponseContext
    {
        IResponseContext Then<T>(Action<T> handler);

        IResponseContext Finally<T>(Action<T> handler);

        IResponseContext Execute();
    }
}
