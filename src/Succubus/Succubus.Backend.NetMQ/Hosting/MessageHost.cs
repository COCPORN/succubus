using System.ComponentModel.Composition;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Succubus.Core.Interfaces;
using Succubus.Hosting.Interfaces;
using Succubus.Serialization;
using NetMQ;

namespace Succubus.Hosting
{
    [Export(typeof(IMessageHost))]
    public class MessageHost : IMessageHost, IHostConfigurator
    {
        Thread serverThread;

        NetMQSocket subscribeSocket;
        NetMQSocket publishSocket;

        readonly ManualResetEvent initializationDone = new ManualResetEvent(false);

        public ManualResetEvent InitializationDone { get { return initializationDone; } }

        public void Start()
        {
            serverThread = new Thread(ServerThread);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        public void Stop()
        {
            poller.Cancel();
            publishSocket.Close();
            publishSocket.Dispose();
            subscribeSocket.Close();
            subscribeSocket.Dispose();

        }

        public MessageHost()
        {
            PublishAddress = "tcp://*:9001";
            SubscribeAddress = "tcp://*:9000";
        }

        private NetMQContext context = null;
        public NetMQContext Context
        {
            get { return context; }
            set { context = value; }
        }

        Poller poller;

        void ServerThread()
        {
            using (context ?? (context = NetMQContext.Create()))
            using (subscribeSocket = context.CreateXSubscriberSocket())
            using (publishSocket = context.CreateXPublisherSocket())
            using (poller = new Poller())
            {
                publishSocket.Bind(PublishAddress);
                subscribeSocket.Bind(SubscribeAddress);
                

                publishSocket.ReceiveReady += publishSocket_ReceiveReady;
                subscribeSocket.ReceiveReady += subscribeSocket_ReceiveReady;

                poller.AddSocket(subscribeSocket);
                poller.AddSocket(publishSocket);                

                InitializationDone.Set();

                while (true) {
                    poller.PollTillCancelled();
                }
            }
        }

        void publishSocket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            var message = e.Socket.ReceiveMessage();
            if (ProcessedMessage != null)
            {
                string address = subscribeSocket.ReceiveString(Encoding.ASCII);
                string typename = subscribeSocket.ReceiveString(Encoding.Unicode);
                string serialized = subscribeSocket.ReceiveString(Encoding.Unicode);

                Type coreType = Type.GetType(typename + ", Succubus.Core");

                object coreMessage = JsonFrame.Deserialize(serialized, coreType);
                if (coreMessage != null)
                {
                    ProcessedMessage(this, new ProcessedMessageEventArgs
                    {
                        Address = address,
                        Message = coreMessage,
                        RawJson = serialized

                    });
                }
            }
            subscribeSocket.SendMessage(message);
        }

        void subscribeSocket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            var message = e.Socket.ReceiveMessage();
            publishSocket.SendMessage(message);
        }


        public string PublishAddress { get; set; }

        public string SubscribeAddress { get; set; }


        public void Initialize()
        {

        }

        public void Initialize(Action<IHostConfigurator> initializationHandler)
        {
            initializationHandler(this);
        }

        public event EventHandler<ProcessedMessageEventArgs> ProcessedMessage;
    }
}




