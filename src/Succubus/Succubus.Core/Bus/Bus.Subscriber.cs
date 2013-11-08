using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Succubus.Hosting;
using Succubus.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ZeroMQ;

namespace Succubus.Core
{
    public partial class Bus
    {
        ManualResetEvent subscriberOnline = new ManualResetEvent(false);
        bool run = true;


        void Subscriber()
        {
            try
            {
                using (subscribeSocket = context.CreateSocket(SocketType.SUB))
                {
                    ConnectSubscriber();
                    subscriberOnline.Set();
                    while (run)
                    {
                        string typename = subscribeSocket.Receive(Encoding.Unicode);
                        string serialized = subscribeSocket.Receive(Encoding.Unicode);
                        Type coreType = Type.GetType(typename + ", Succubus.Core");

                        object coreMessage = JsonFrame.Deserlialize(serialized, coreType);

                        var synchronousFrame = coreMessage as SynchronousMessageFrame;
                        var eventFrame = coreMessage as EventMessageFrame;
                        if (synchronousFrame != null)
                        {
                            ProcessSynchronousMessages(synchronousFrame);
                        }
                        else if (eventFrame != null)
                        {
                            ProcessEvents(eventFrame);
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }



        private void ProcessSynchronousMessages(SynchronousMessageFrame synchronousFrame)
        {
            Type type = Type.GetType(synchronousFrame.EmbeddedType);
            object message = JsonFrame.Deserlialize(synchronousFrame.Message, type);

            ProcessReplyHandlers(synchronousFrame, type, message);

            SynchronizationContext ctx = ProcessSynchronousHandlers(synchronousFrame, message);
        }



        private SynchronizationContext ProcessSynchronousHandlers(SynchronousMessageFrame synchronousFrame, object message)
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

                }
                catch
                {
                }
            }


            return ctx;
        }

        private void ProcessReplyHandlers(SynchronousMessageFrame synchronousFrame, Type type, object message)
        {
            List<Func<object, object>> handlers = null;

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
                        Func<object, object> handler = replyHandler;
                        Task.Factory.StartNew(() =>
                        {
                            var response = handler(message);
                            var framedResponse = FrameResponseSynchronously(synchronousFrame, response, synchronousFrame.CorrelationId);
                            ObjectPublish(framedResponse);
                        });
                    }
                }
                catch { }
            }
        }

    }
}
