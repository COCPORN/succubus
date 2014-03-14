using System.Runtime.Remoting.Messaging;
using System.Threading;
using Succubus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Core
{
    public partial class Bus
    {
        /// <summary>
        /// These are used with On to handle events.
        /// </summary>
        readonly Dictionary<Type, List<EventBlock>> eventHandlers = new Dictionary<Type, List<EventBlock>>();

        public void Publish<T>(T request, string address = null, Action<Action> marshal = null)
        {
            Action execute = () => Transport.ObjectPublish(FrameEvent(request), address ?? "__BROADCAST");

            if (marshal == null)
            {
                execute();
            }
            else
            {
                marshal(execute);
            }
        }


        public IResponseContext On<T>(Action<T> handler, string address = null, Action<Action> marshal = null)
        {
            if (typeof (T) == typeof (IMessageFrame))
            {
                var myHandler = new Action<IMessageFrame>(response => handler((T) response));
                frameHandlers.Add(new FrameBlock() { Handler = myHandler, Marshal = marshal });
                return new Bus.ResponseContext(this);
            }
            else
            {
                if (typeof (T).BaseType == null)
                {
                    SubscriptionManager.SubscribeAll();
                }
                else
                {
                    SetupSubscriber(address);
                }

                var myHandler = new Action<object>(response => handler((T) response));
                lock (eventHandlers)
                {
                    List<EventBlock> handlers;
                    if (eventHandlers.TryGetValue(typeof (T), out handlers) == false)
                    {
                        handlers = new List<EventBlock>();
                        eventHandlers.Add(typeof (T), handlers);
                    }

                    handlers.Add(new EventBlock() { Handler = myHandler, Address = address, Marshal = marshal });
                }

                return new Bus.ResponseContext(this);
            }
        }

        private void SetupSubscriber(string address)
        {
            if (address == null)
            {
                SubscriptionManager.SubscribeAll();
            }
            else
            {
                SubscriptionManager.Subscribe(address);
            }
        }

        private bool replySubscriptionSetup = false;
        object replySubscriptionLock = new object();
        void SetupReplySubscription()
        {
            lock (replySubscriptionLock)
            {
                if (replySubscriptionSetup == true) return;
                else
                {
                    SetupSubscriber("__REPLY");

                    replySubscriptionSetup = true;
                }
            }

        }

        public void ProcessEvents(MessageFrames.Event eventFrame, string address)
        {
            FrameMessage(eventFrame);

            //Type type = Type.GetType(eventFrame.EmbeddedType);
            //Type eventType = Type.GetType(eventFrame.EmbeddedType);
            object message = eventFrame.Message;
            if (message == null) return;

            Type eventType = message.GetType();
            IEnumerable<Type> interfaces = eventType.GetInterfaces();

            List<EventBlock> handlers = new List<EventBlock>();

            lock (eventHandlers)
            {
                while (eventType != null)
                {
                    List<EventBlock> localHandlers = new List<EventBlock>();
                    if (eventHandlers.TryGetValue(eventType, out localHandlers))
                    {
                        handlers.AddRange(localHandlers);
                    }
                    eventType = eventType.BaseType;
                }
                foreach (var @interface in interfaces)
                {
                    List<EventBlock> localHandlers = new List<EventBlock>();
                    if (eventHandlers.TryGetValue(@interface, out localHandlers))
                    {
                        handlers.AddRange(localHandlers);
                    }
                }
            }

            // TODO: This has a potential race condition in where
            // handlers are added to/subtracted from while iterating on it
            // TODO: DOES IT REALLY? I don't see that now, but I might in the future
            foreach (var eventHandler in handlers)
            {
                var handler = eventHandler;
                if (handler.Address == null || handler.Address == address)
                {
                    try
                    {
                        if (handler.Marshal == null)
                        {
                            Task.Factory.StartNew(() => handler.Handler(message));
                        }
                        else
                        {
                            handler.Marshal(() => handler.Handler(message));
                        }
                    }
                    catch (Exception ex)
                    {
                        RaiseExceptionEvent(ex);
                    }
                }
            }
        }

        public void ProcessCatchAllEvents(MessageFrames.Synchronous eventFrame, string address)
        {

            FrameMessage(eventFrame);
            List<EventBlock> handlers = null;

            lock (eventHandlers)
            {

                if (eventHandlers.TryGetValue(typeof(object), out handlers) == false)
                {
                    return;
                }

            }

            Type type = Type.GetType(eventFrame.EmbeddedType);
            Type messageType = Type.GetType(eventFrame.EmbeddedType);
            object message = eventFrame.Message;

            if (type == null || messageType == null || message == null) return;

            // TODO: This has a potential race condition in where
            // handlers are added to/subtracted from while iterating on it
            if (handlers != null)
            {
                foreach (var eventHandler in handlers)
                {
                    var handler = eventHandler;
                    if (handler.Address == null || handler.Address == address)
                    {
                        try
                        {
                            if (handler.Marshal == null)
                            {
                                Task.Factory.StartNew(() => handler.Handler(message));
                            }
                            else
                            {
                                handler.Marshal(() => handler.Handler(message));
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
    }

    internal class EventBlock
    {
        internal string Address;
        internal Action<object> Handler;
        internal Action<Action> Marshal;
    }
}
