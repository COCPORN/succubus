﻿using System.Security.AccessControl;
using Succubus.Interfaces.ResponseContexts;
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
        Dictionary<Type, List<Action<object>>> eventHandlers = new Dictionary<Type, List<Action<object>>>();

        public void Publish<T>(T request)
        {
            ObjectPublish(FrameEvent(request));
        }        

        public IResponseContext On<T>(Action<T> handler)
        {   
            Action<object> myHandler = new Action<object>(response => handler((T)response));
            List<Action<object>> handlers;
            lock (eventHandlers)
            {
                if (eventHandlers.TryGetValue(typeof (T), out handlers) == false)
                {
                    handlers = new List<Action<object>>();
                    eventHandlers.Add(typeof (T), handlers);
                }

                handlers.Add(myHandler);
            }
            return new ResponseContext(this);
        }

        private void ProcessEvents(EventMessageFrame eventFrame)
        {
            Type type = Type.GetType(eventFrame.EmbeddedType);
            Type eventType = Type.GetType(eventFrame.EmbeddedType);
            object message = JsonFrame.Deserlialize(eventFrame.Message, type);

            if (type == null || eventType == null || message == null) return;

            List<Action<object>> handlers = null;
            
            lock (eventHandlers)
            {
                eventHandlers.TryGetValue(eventType, out handlers);
            }

            // TODO: This has a potential race condition in where
            // handlers are added to/subtracted from while iterating on it
            if (handlers != null)
            {
                foreach (var eventHandler in handlers)
                {
                    eventHandler(message);
                }
            }
        }
    }
}
