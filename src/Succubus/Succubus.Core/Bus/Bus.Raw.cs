using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Succubus.Core.Interfaces;
using System.Threading;

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

        public void OnRawMessage(Action<object> handler, Action<Action> marshal = null)
        {
            SubscriptionManager.SubscribeAll();
            rawMessageHandlers.Add(new RawMessageBlock() { Handler = handler, Marshal = marshal });                        
        }

        public void OnRawData(Action<string> handler, Action<Action> marshal = null)
        {
            SubscriptionManager.SubscribeAll();
            rawDataHandlers.Add(new RawDataBlock { Handler = handler, Marshal = marshal });            
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
                        new Thread(new ThreadStart(delegate()
                        {
                            try
                            {
                                handler.Handler(o);
                            }
                            catch (AggregateException ex)
                            {
                                ex.Handle((x) =>
                                {
                                    RaiseExceptionEvent(x);
                                    return true;
                                });
                            }
                            catch (Exception ex)
                            {
                                RaiseExceptionEvent(ex);
                            }
                        })).Start();                      
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
                        new Thread(new ThreadStart(delegate()
                        {
                            try
                            {
                                handler.Handler(data);
                            }
                            catch (AggregateException ex)
                            {
                                ex.Handle((x) =>
                                {
                                    RaiseExceptionEvent(x);
                                    return true;
                                });
                            }
                            catch (Exception ex)
                            {
                                RaiseExceptionEvent(ex);
                            }
                        })).Start();
                        
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
