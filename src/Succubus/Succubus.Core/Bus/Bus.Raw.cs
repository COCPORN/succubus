using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Succubus.Core.Interfaces;

namespace Succubus.Core
{
    internal class RawBlock
    {
        internal Action<object> Handler;
        internal Action<Action> Marshal;
    }

    public partial class Bus
    {
        private readonly List<RawBlock> rawHandlers = new List<RawBlock>();

        public IResponseContext OnRaw(Action<object> handler, Action<Action> marshal = null)
        {
            rawHandlers.Add(new RawBlock() { Handler = handler, Marshal = marshal });
            return (IResponseContext)this;
        }

        public void RawMessage(object o)
        {
            foreach (var eventHandler in rawHandlers)
            {
                var handler = eventHandler;

                try
                {
                    if (handler.Marshal == null)
                    {
                        Task.Factory.StartNew(() => handler.Handler(o));
                    }
                    else
                    {
                        handler.Marshal(() => handler.Handler(o));
                    }                    
                }
                catch (Exception ex)
                {
                    RaiseExceptionEvent(ex);
                }

            }
        }
    }
}
