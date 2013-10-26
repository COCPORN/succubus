using Succubus.Interfaces.ResponseContexts;
using Succubus.Serialization;
using System;
using System.Collections.Generic;

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
            if (eventHandlers.TryGetValue(typeof(T), out handlers) == false) {
                handlers = new List<Action<object>>();
                eventHandlers.Add(typeof(T), handlers);
            }

            handlers.Add(myHandler);
            return new ResponseContext(this);
        }

        private void ProcessEvents(EventMessageFrame eventFrame)
        {
            Type type = Type.GetType(eventFrame.EmbeddedType);
            Type eventType = Type.GetType(eventFrame.EmbeddedType);
            object message = JsonFrame.Deserlialize(eventFrame.Message, type);

            List<Action<object>> handlers;

            if (eventHandlers.TryGetValue(eventType, out handlers))
            {
                foreach (var eventHandler in handlers)
                {
                    eventHandler(message);
                }
            }
        }
    }
}
