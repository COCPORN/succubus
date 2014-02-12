using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Succubus.Core.Interfaces;

namespace Succubus.Backend.Loopback
{
    public class Transport : ITransport, ISubscriptionManager
    {
        static List<Transport> transports = new List<Transport>();

        public List<string> SubscriptionList = new List<string>();

        public void ObjectPublish(object message, string address)
        {
   
            lock (transports)
            {
                foreach (var transport in transports)
                {
                    bool receive = false;
                    foreach (var subAddress in transport.SubscriptionList)
                    {
                        if (address.StartsWith(subAddress)) receive = true;
                    }
                    if (receive == false) continue;
                    var synchronousFrame = message as Core.MessageFrames.Synchronous;
                    var eventFrame = message as Core.MessageFrames.Event;
                    if (synchronousFrame != null)
                    {                    
                        transport.Bridge.ProcessSynchronousMessages(synchronousFrame, address);
                        transport.Bridge.ProcessCatchAllEvents(synchronousFrame, address);
                    }
                    else if (eventFrame != null)
                    {
                        transport.Bridge.ProcessEvents(eventFrame, address);
                    }
                }
            }

        }



        public void Subscribe(string address)
        {
            SubscriptionList.Add(address);
        }

        public void SubscribeAll()
        {
            SubscriptionList.Add("");
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
    }
}
