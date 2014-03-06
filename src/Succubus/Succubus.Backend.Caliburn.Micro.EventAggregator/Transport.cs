using System;
using Caliburn.Micro;
using Succubus.Core.Interfaces;
using Succubus.Core.MessageFrames;

namespace Succubus.Backend.Caliburn.Micro.EventAggregator
{
    public class Transport : ITransport, ISubscriptionManager, ICorrelationIdProvider
    {
        public IEventAggregator EventAggregator { get; set; }

        public ITransportBridge Bridge { get; set; }

        public void Initialize()
        {
            EventAggregator.Subscribe(this);
        }

        public void Handle(object o)
        {
            Bridge.ProcessEvents(new Event()
            {
                Message = o
            }, null);
        }

        public void ObjectPublish(object message, string address, Action<System.Action> marshal = null)
        {
            if (marshal == null) EventAggregator.PublishOnUIThread(message);
            else EventAggregator.Publish(message, marshal);
        }
      
        public void Subscribe(string address)
        {
          
        }

        public void SubscribeAll()
        {
          
        }

        public string CreateCorrelationId(object o)
        {
            throw new InvalidOperationException("EventAggregator transport does not support synchronous operations");
        }
    }
}