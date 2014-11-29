using System.Net.Sockets;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Succubus.Core.Diagnostics;
using Succubus.Core.Interfaces;

namespace Succubus.Core
{
    public partial class Bus : ITransportBridge
    {



        public void ProcessSynchronousMessages(MessageFrames.Synchronous synchronousFrame, string address)
        {
            object message = synchronousFrame.Message;

            if (message == null) return;
            Type type = message.GetType();

            ProcessReplyHandlers(synchronousFrame, type, message, address);

            SynchronizationContext ctx = ProcessSynchronousHandlers(synchronousFrame, message, address);
        }



        private SynchronizationContext ProcessSynchronousHandlers(MessageFrames.Synchronous synchronousFrame, object message, string address)
        {
            SynchronizationContext ctx;

            lock (synchronizationContexts)
            {
                synchronizationContexts.TryGetValue(synchronousFrame.CorrelationId, out ctx);
            }
            if (ctx != null)
            {
                try
                {
                    if (ctx.ResolveFor(message) == true)
                    {

                        lock (synchronizationContexts)
                        {
                            synchronizationContexts.Remove(synchronousFrame.CorrelationId);
                            timeoutHandler.RemoveTimeout(synchronousFrame.CorrelationId);
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

                foreach (var replyHandler in handlers)
                {
                    SynchronousBlock handler = replyHandler;
                    if (handler.Address == "__BROADCAST" || handler.Address == "__REPLY" || handler.Address == address)
                    {
                        try
                        {
                            Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    var response = handler.Handler(message);
                                    if (response != null)
                                    {
                                        var framedResponse = FrameResponseSynchronously(synchronousFrame, response,
                                            synchronousFrame.CorrelationId);
                                        Transport.BusPublish(framedResponse, "__REPLY");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    RaiseExceptionEvent(ex);
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            RaiseExceptionEvent(ex);
                        }

                    }
                }

            }
        }

    }
}
