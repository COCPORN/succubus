using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Succubus.Core.Interfaces;

namespace Succubus.Core
{
    internal class RawMessageBlock
    {
        internal Action<object> Handler;
        internal Action<Action> Marshal;
    }

    internal class RawDataBlock
    {
        internal Action<string> Handler;
        internal Action<Action> Marshal;
    }

    public partial class Bus
    {
        private readonly List<RawMessageBlock> rawMessageHandlers = new List<RawMessageBlock>();
        private readonly List<RawDataBlock> rawDataHandlers = new List<RawDataBlock>();

        public IResponseContext OnRawMessage(Action<object> handler, Action<Action> marshal = null)
        {
            SubscriptionManager.SubscribeAll();
            rawMessageHandlers.Add(new RawMessageBlock() { Handler = handler, Marshal = marshal });            
            return new Bus.ResponseContext(this);
        }

        public IResponseContext OnRawData(Action<string> handler, Action<Action> marshal = null)
        {
            SubscriptionManager.SubscribeAll();
            rawDataHandlers.Add(new RawDataBlock { Handler = handler, Marshal = marshal });
            return new Bus.ResponseContext(this);
        }

        public void RawMessage(object o)
        {
            foreach (var eventHandler in rawMessageHandlers)
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

        public void RawData(string data)
        {
            foreach (var eventHandler in rawDataHandlers)
            {
                var handler = eventHandler;

                try
                {
                    if (handler.Marshal == null)
                    {
                        Task.Factory.StartNew(() => handler.Handler(data));
                    }
                    else
                    {
                        handler.Marshal(() => handler.Handler(data));
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
