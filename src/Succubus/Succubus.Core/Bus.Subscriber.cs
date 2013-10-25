using Succubus.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
                Console.WriteLine("Subscriber caught exception: {0}", ex.ToString());
                Console.ReadLine();
            }
        }

        private void ProcessEvents(EventMessageFrame eventFrame)
        {
            Type type = Type.GetType(eventFrame.EmbeddedType);
            Type eventType = Type.GetType(eventFrame.EmbeddedType);
            object message = JsonFrame.Deserlialize(eventFrame.Message, type);

            Action<object> eventHandler;

            if (eventHandlers.TryGetValue(eventType, out eventHandler))
            {
                eventHandler(message);
            }
        }

        private void ProcessSynchronousMessages(SynchronousMessageFrame synchronousFrame)
        {
            Type type = Type.GetType(synchronousFrame.EmbeddedType);
            Type requestType = Type.GetType(synchronousFrame.RequestType);
            object message = JsonFrame.Deserlialize(synchronousFrame.Message, type);

            ProcessReplies(synchronousFrame, type, message);

            SynchronizationContext ctx = ProcessTransientHandlers(synchronousFrame, message);

            //ctx = ProcessStaticHandlers(synchronousFrame, requestType, message, ctx);
        }

        private SynchronizationContext ProcessStaticHandlers(SynchronousMessageFrame synchronousFrame, Type requestType, object message, SynchronizationContext ctx)
        {
            Dictionary<Guid, SynchronizationContext> staticContexts;
            if (staticSynchronizationContexts.TryGetValue(requestType, out staticContexts))
            {
                try
                {
                    if (staticContexts.TryGetValue(synchronousFrame.CorrelationId, out ctx))
                    {
                        if (ctx.ResolveFor(message))
                        {
                            staticContexts.Remove(synchronousFrame.CorrelationId);
                        }
                    }
                }
                catch { }
            }
            return ctx;
        }

        private SynchronizationContext ProcessTransientHandlers(SynchronousMessageFrame synchronousFrame, object message)
        {
            SynchronizationContext ctx = null;
            if (transientSynchronizationContexts.TryGetValue(synchronousFrame.CorrelationId, out ctx))
            {
                try
                {
                    if (ctx.ResolveFor(message) == true)
                    {
                        transientSynchronizationContexts.Remove(synchronousFrame.CorrelationId);
                    }
                }
                catch { }
            }
            return ctx;
        }

        private void ProcessReplies(SynchronousMessageFrame synchronousFrame, Type type, object message)
        {
            Func<object, object> replyHandler;
            if (replyHandlers.TryGetValue(type, out replyHandler))
            {
                try
                {
                    var response = replyHandler(message);
                    var framedResponse = FrameResponseSynchronously(synchronousFrame, response, synchronousFrame.CorrelationId);
                    ObjectPublish(framedResponse);
                }
                catch { }
            }
        }

    }
}
