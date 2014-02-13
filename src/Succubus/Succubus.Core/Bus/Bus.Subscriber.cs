using System.Net.Sockets;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Succubus.Core.Interfaces;

namespace Succubus.Core
{
    public partial class Bus : ITransportBridge
    {
     


        public void ProcessSynchronousMessages(MessageFrames.Synchronous synchronousFrame, string address)
        {
     

            Type type = Type.GetType(synchronousFrame.EmbeddedType);
            object message = synchronousFrame.Message;

            ProcessReplyHandlers(synchronousFrame, type, message, address);

            SynchronizationContext ctx = ProcessSynchronousHandlers(synchronousFrame, message, address);
        }



        private SynchronizationContext ProcessSynchronousHandlers(MessageFrames.Synchronous synchronousFrame, object message, string address)
        {
         

            SynchronizationContext ctx;

            lock (synchronizationContexts)
            {
                synchronizationContexts.TryGetValue(synchronousFrame.CorrelationId, out ctx);
                if (ctx == null)
                {
                    lock (deferredRequestTypes)
                    {
                        if (deferredRequestTypes.Contains(message.GetType()))
                        {
                            ctx = InstantiatePrototype(message, null, 60000, synchronousFrame.CorrelationId);
                            if (ctx != null)
                            {
                                ctx.Request = message;
                                
                                lock (deferredWaitHandles)
                                {
                                    ManualResetEvent handle = null;
                                    if (deferredWaitHandles.TryGetValue(synchronousFrame.CorrelationId, out handle))
                                    {
                                        handle.Set();
                                    }
                                }
                            }
                            else throw new InvalidOperationException("Unable to instantiate context from prototype");
                        }
                    }
                }
            }
            if (ctx != null)
            {
                try
                {
                    if (ctx.ResolveFor(message) == true)
                    {
                        if (ctx.ContextType != ContextType.Deferred)
                        {
                            lock (synchronizationContexts)
                            {
                                synchronizationContexts.Remove(synchronousFrame.CorrelationId);
                                timeoutHandler.RemoveTimeout(synchronousFrame.CorrelationId);
                            }
                        }
                    }
                    else if (ctx.TimedOut == true)
                    {
                        lock (synchronizationContexts)
                        {
                            synchronizationContexts.Remove(synchronousFrame.CorrelationId);
                            timeoutHandler.RemoveTimeout(synchronousFrame.CorrelationId);
                        }
                    }

                }
                catch
                {
                }
            }


            return ctx;
        }

        private void ProcessReplyHandlers(MessageFrames.Synchronous synchronousFrame, Type type, object message, string address)
        {
        
            List<SynchronousBlock> handlers = null;

            lock (replyHandlers)
            {
                replyHandlers.TryGetValue(type, out handlers);
            }
            if (handlers != null)
            {
                try
                {
                    foreach (var replyHandler in handlers)
                    {
                        SynchronousBlock handler = replyHandler;
                        if (handler.Address == address)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                var response = handler.Handler(message);
                                var framedResponse = FrameResponseSynchronously(synchronousFrame, response,
                                    synchronousFrame.CorrelationId);
                                Transport.ObjectPublish(framedResponse, "__REPLY");
                            });
                        }
                    }
                }
                catch { }
            }
        }

    }
}
