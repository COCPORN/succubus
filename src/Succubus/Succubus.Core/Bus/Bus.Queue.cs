using System;
using System.Collections.Generic;
using Succubus.Core.Interfaces;

namespace Succubus.Core
{
    public partial class Bus
    {

        public void Enqueue<T>(T request, string address = null, Action<Action> marshal = null)
        {
            Action execute = () => Transport.QueuePublish(FrameWorkItem(request), address ?? "__QUEUE");

            if (marshal == null)
            {
                execute();
            }
            else
            {
                marshal(execute);
            }
        }


        public void Dequeue<T>(Action<T> handler, string address = null, Action<Action> marshal = null)
        {
            if (typeof(T) == typeof(IMessageFrame))
            {
                var myHandler = new Action<IMessageFrame>(response => handler((T)response));
                frameHandlers.Add(new FrameBlock() { Handler = myHandler, Marshal = marshal });                
            }
            else
            {
                if (typeof(T).BaseType == null)
                {
                    SubscriptionManager.SubscribeAll();
                }
                else
                {
                    SetupSubscriber(address);
                }

                var myHandler = new Action<object>(response => handler((T)response));
                lock (eventHandlers)
                {
                    List<EventBlock> handlers;
                    if (eventHandlers.TryGetValue(typeof(T), out handlers) == false)
                    {
                        handlers = new List<EventBlock>();
                        eventHandlers.Add(typeof(T), handlers);
                    }

                    handlers.Add(new EventBlock() { Handler = myHandler, Address = address, Marshal = marshal });
                }                
            }
        }
         
    }
}