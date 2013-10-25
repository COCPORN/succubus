using Succubus.Interfaces.ResponseContexts;
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
        Dictionary<Type, Action<object>> eventHandlers = new Dictionary<Type, Action<object>>();

        public void Publish<T>(T request)
        {
            ObjectPublish(FrameEvent(request));
        }        

        public IResponseContext On<T>(Action<T> handler)
        {
            if (eventHandlers.ContainsKey(typeof(T)))
            {
                throw new ArgumentException("Type already has a handler");
            }
            Action<object> myHandler = new Action<object>(response => handler((T)response));
            eventHandlers.Add(typeof(T), myHandler);
            return new ResponseContext(this);
        }
       
    }
}
