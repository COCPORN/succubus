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
                Console.WriteLine("Subscriber caught exception: {0}", ex.ToString());
                Console.ReadLine();
            }
        }



        private void ProcessSynchronousMessages(SynchronousMessageFrame synchronousFrame)
        {
            Type type = Type.GetType(synchronousFrame.EmbeddedType);
            Type requestType = Type.GetType(synchronousFrame.RequestType);
            object message = JsonFrame.Deserlialize(synchronousFrame.Message, type);

            ProcessReplies(synchronousFrame, type, message);

            SynchronizationContext ctx = ProcessSynchrohousHandlers(synchronousFrame, message);            
        }

      

        private SynchronizationContext ProcessSynchrohousHandlers(SynchronousMessageFrame synchronousFrame, object message)
        {
            SynchronizationContext ctx = null;

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
                        }
                    }
                }                
                catch { }

            }
            return ctx;
        }

        private void ProcessReplies(SynchronousMessageFrame synchronousFrame, Type type, object message)
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
                        var response = replyHandler(message);
                        var framedResponse = FrameResponseSynchronously(synchronousFrame, response, synchronousFrame.CorrelationId);
                        ObjectPublish(framedResponse);
                    }
                }
                catch { }
            }
        }

    }
}
