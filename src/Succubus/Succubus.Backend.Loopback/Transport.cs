using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Succubus.Core.Interfaces;

namespace Succubus.Backend.Loopback
{
    public class Transport : ITransport, ISubscriptionManager, ICorrelationIdProvider, ILoopbackConfigurator
    {
        static List<Transport> transports = new List<Transport>();

        public List<string> SubscriptionList = new List<string>();

        public bool ReportRaw { get; set; }

        public void BusPublish(object message, string address)
        {
   
            lock (transports)
            {
                foreach (var transport in transports)
                {
                   

                    bool receive = false;
                    lock (transport.SubscriptionList)
                    {
                        foreach (var subAddress in transport.SubscriptionList)
                        {
                            if (address.StartsWith(subAddress)) receive = true;
                        }
                    }
                    if (receive == false) continue;
                    var synchronousFrame = message as Core.MessageFrames.Synchronous;
                    var eventFrame = message as Core.MessageFrames.Event;
                    if (synchronousFrame != null)
                    {                    
                        transport.Bridge.ProcessSynchronousMessages(synchronousFrame, address);
                        transport.Bridge.ProcessCatchAllEvents(synchronousFrame, address);
                        if (transport.ReportRaw)
                        {
                            transport.Bridge.RawMessage(synchronousFrame);
                        }
                    }
                    else if (eventFrame != null)
                    {
                        transport.Bridge.ProcessEvents(eventFrame, address);
                        if (transport.ReportRaw)
                        {
                            transport.Bridge.RawMessage(eventFrame);
                        }
                    }
                }
            }

        }

        public void BusPublish(object message, string address, Action<Action> marshal)
        {
            if (marshal == null) BusPublish(message, address);
            else marshal(() => BusPublish(message, address));           
        }

        public void Subscribe(string address)
        {
            lock (SubscriptionList)
            {
                if (SubscriptionList.Contains(address) == false)
                {
                    SubscriptionList.Add(address);
                }
            }
        }

        public void SubscribeAll()
        {
            lock (SubscriptionList)
            {
                if (SubscriptionList.Contains("") == false)
                {
                    SubscriptionList.Add("");
                }
            }
        }

        public ITransportBridge Bridge { get; set; }

        public void Initialize(bool clear = false)
        {
            lock (transports)
            {
                if (clear) transports.Clear();
                transports.Add(this);
            }

        }

        public string CreateCorrelationId(object o)
        {
            return Guid.NewGuid().ToString();
        }

        public void QueuePublish(object message, string address, Action<Action> marshal = null)
        {
            throw new NotImplementedException();
        }
    }
}
