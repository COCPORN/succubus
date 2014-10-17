using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Succubus.Core.Interfaces;
using Succubus.Serialization;
using ZeroMQ;

namespace Succubus.Backend.ZeroMQ
{
    public class Transport : ITransport, IZeroMQConfigurator, ISubscriptionManager, ICorrelationIdProvider
    {
        public void SetupSubscriber(string address)
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

        bool reportRaw = false;
        public bool ReportRaw
        {
            get
            {
                return reportRaw;
            }
            set
            {
                if (value != reportRaw)
                {
                    if (value == true)
                    {
                        SubscribeAll();
                    }
                    reportRaw = value;
                }
            }
        }

        public void SubscribeAll()
        {
            subscribeSocket.SubscribeAll();
        }

        public void Subscribe(string address)
        {
            //if (subscribeSetup == false) SubscribeAll();
            //return;
            subscribeSocket.Subscribe(Encoding.ASCII.GetBytes(address));

            // Make sure the reply channel is fully registered on the host before contiuing.
            // The reply channel is only setup once per bus instance, so this sleep will only incur
            // once
            if (address.StartsWith("__REPLY")) Thread.Sleep(100); 
            
        }

        ManualResetEvent subscriberOnline = new ManualResetEvent(false);

        public ManualResetEvent SubscriberOnline
        {
            get { return subscriberOnline; }            
        }

        bool run = true;
        public ITransportBridge Bridge { get; set; }
        public IBusConfigurator Configurator { get; set; }


        #region Threading

        private Thread subscriberThread;

        #endregion



        private bool initialized = false;
        public void Initialize()
        {
            lock (this)
            {
                if (initialized == true)
                {
                    throw new InvalidOperationException("Transport is already initialized");
                }
                initialized = true;
            }

            ConnectPublisher();

            subscriberThread = new Thread(Subscriber) { IsBackground = true };
            subscriberThread.Start();

        }

        #region ØMQ


        private ZmqContext context;
        private ZmqSocket publishSocket;
        private ZmqSocket subscribeSocket;

        #endregion

        void Subscriber()
        {
            try
            {
                using (subscribeSocket = context.CreateSocket(SocketType.SUB))
                {
                    ConnectSubscriber();
                    subscriberOnline.Set();
                    while (run)
                    {
                        string address = subscribeSocket.Receive(Encoding.ASCII);
                        string typename = subscribeSocket.Receive(Encoding.Unicode);
                        string serialized = subscribeSocket.Receive(Encoding.Unicode);
                        Type coreType = Type.GetType(typename + ", Succubus.Core");

                        if (reportRaw == true)
                        {
                            Bridge.RawData(serialized);
                        }

                        object coreMessage = JsonFrame.Deserialize(serialized, coreType);
                        
                        if (coreMessage == null)
                        {
                            Bridge.UnableToCreateMessage(
                                new Exception(
                                    String.Format(
                                        "Unable to create message from: Address: {0} Typename: {1} Serialized: {2}",
                                        address, typename, serialized)));
                        }

                        var synchronousFrame = coreMessage as Core.MessageFrames.Synchronous;
                        var eventFrame = coreMessage as Core.MessageFrames.Event;
                        if (synchronousFrame != null)
                        {                            
                            Bridge.ProcessSynchronousMessages(synchronousFrame, address);
                            Bridge.ProcessCatchAllEvents(synchronousFrame, address);
                            if (ReportRaw)
                            {
                                Bridge.RawMessage(synchronousFrame);
                            }
                        }
                        else if (eventFrame != null)
                        {
                            Bridge.ProcessEvents(eventFrame, address);
                            if (ReportRaw)
                            {
                                Bridge.RawMessage(eventFrame);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Bridge.GeneralTransportException(ex);
            }
        }


        public string Network { get; set; }

        public string PublishAddress { get; set; }

        public string SubscribeAddress { get; set; }

        public void GetFromConfigurationFile()
        {
            PublishAddress = ConfigurationManager.AppSettings["Succubus.PublishAddress"];
            SubscribeAddress = ConfigurationManager.AppSettings["Succubus.SubscribeAddress"];


            Network = ConfigurationManager.AppSettings["Succubus.Network"];
        }

        public Transport()
        {
            PublishAddress = "tcp://localhost:9000";
            SubscribeAddress = "tcp://localhost:9001";
            context = ZmqContext.Create();
        }

        public ZmqContext Context { get { return context; } set { context = value; } }

        public void BusPublish(object message, string address, Action<Action> marshal)
        {
            if (marshal == null) BusPublish(message, address);
            else marshal(() => BusPublish(message, address));          
        }


        public void QueuePublish(object message, string address, Action<Action> marshal)
        {
            
        }


        public void BusPublish(object message, string address)
        {
            lock (publishSocket)
            {
                publishSocket.SendMore(Encoding.ASCII.GetBytes(address));
                var typeIndentifier = message.GetType().ToString();
                publishSocket.SendMore(typeIndentifier, Encoding.Unicode);
                var serialized = JsonFrame.Serialize(message);
                publishSocket.Send(serialized, Encoding.Unicode);
            }
        }

        private void ConnectPublisher()
        {
            publishSocket = context.CreateSocket(SocketType.PUB);
            publishSocket.Connect(PublishAddress);
        }

        private void ConnectSubscriber()
        {
            subscribeSocket.Connect(SubscribeAddress);
        }


        public string CreateCorrelationId(object o)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
