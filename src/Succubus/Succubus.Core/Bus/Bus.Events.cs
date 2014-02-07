﻿using System.Security.AccessControl;
using Succubus.Core.Interfaces;
using Succubus.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Publish<T>(T request, string address = null)
        {
            ObjectPublish(FrameEvent(request, address ?? "__BROADCAST"), address ?? "__BROADCAST");
        }

        public IResponseContext On<T>(Action<T> handler, string address = null)
        {
            SetupSubscriber(address);

            var myHandler = new Action<object>(response => handler((T)response));
            lock (eventHandlers)
            {
                List<EventBlock> handlers;
                if (eventHandlers.TryGetValue(typeof(T), out handlers) == false)
                {
                    handlers = new List<EventBlock>();
                    eventHandlers.Add(typeof(T), handlers);
                }

                handlers.Add(new EventBlock() { Handler = myHandler, Address = address ?? "__BROADCAST" });
            }
            return new Bus.ResponseContext(this);
        }

        private void SetupSubscriber(string address)
        {
            if (address == null)
            {
                subscribeSocket.SubscribeAll();
            }
            else
            {
                subscribeSocket.Subscribe(Encoding.ASCII.GetBytes(address));
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

        private void ProcessEvents(MessageFrames.Event eventFrame, string address)
        {
          

            Type type = Type.GetType(eventFrame.EmbeddedType);
            Type eventType = Type.GetType(eventFrame.EmbeddedType);
            IEnumerable<Type> interfaces = eventType.GetInterfaces();
            object message = JsonFrame.Deserlialize(eventFrame.Message, type);

            if (type == null || eventType == null || message == null) return;

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
                if (handler.Address == address)
                {
                    Task.Factory.StartNew(() => handler.Handler(message));
                }
            }
        }

        private void ProcessCatchAllEvents(MessageFrames.Synchronous eventFrame, string address)
        {
         
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
            object message = JsonFrame.Deserlialize(eventFrame.Message, type);

            if (type == null || messageType == null || message == null) return;

            // TODO: This has a potential race condition in where
            // handlers are added to/subtracted from while iterating on it
            if (handlers != null)
            {
                foreach (var eventHandler in handlers)
                {
                    var handler = eventHandler;
                    if (handler.Address == address)
                    {
                        Task.Factory.StartNew(() => handler.Handler(message));
                    }
                }
            }
        }
    }

    internal class EventBlock
    {
        internal string Address;
        internal Action<object> Handler;
    }
}
