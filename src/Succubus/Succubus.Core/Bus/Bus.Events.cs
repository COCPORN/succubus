using System.Security.AccessControl;
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
        readonly Dictionary<Type, List<Action<object>>> eventHandlers = new Dictionary<Type, List<Action<object>>>();

        public void Publish<T>(T request)
        {
            ObjectPublish(FrameEvent(request));
        }

        public IResponseContext On<T>(Action<T> handler)
        {
            var myHandler = new Action<object>(response => handler((T)response));
            lock (eventHandlers)
            {
                List<Action<object>> handlers;
                if (eventHandlers.TryGetValue(typeof(T), out handlers) == false)
                {
                    handlers = new List<Action<object>>();
                    eventHandlers.Add(typeof(T), handlers);
                }

                handlers.Add(myHandler);
            }
            return new Bus.ResponseContext(this);
        }

        private void ProcessEvents(MessageFrames.Event eventFrame)
        {
            Type type = Type.GetType(eventFrame.EmbeddedType);
            Type eventType = Type.GetType(eventFrame.EmbeddedType);
            IEnumerable<Type> interfaces = eventType.GetInterfaces();
            object message = JsonFrame.Deserlialize(eventFrame.Message, type);

            if (type == null || eventType == null || message == null) return;

            List<Action<object>> handlers = new List<Action<object>>();

            lock (eventHandlers)
            {
                while (eventType != null)
                {
                    List<Action<object>> localHandlers = new List<Action<object>>();
                    if (eventHandlers.TryGetValue(eventType, out localHandlers))
                    {
                        handlers.AddRange(localHandlers);
                    }
                    eventType = eventType.BaseType;
                }
                foreach (var @interface in interfaces)
                {
                    List<Action<object>> localHandlers = new List<Action<object>>();
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
                Task.Factory.StartNew(() => handler(message));
            }
        }

        private void ProcessCatchAllEvents(MessageFrames.Synchronous eventFrame)
        {
            List<Action<object>> handlers = null;

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
                    Task.Factory.StartNew(() => handler(message));
                }
            }
        }
    }
}
